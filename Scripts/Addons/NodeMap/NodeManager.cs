using UnityEngine;
using UnityEngine.Events;
using JSNodeMap;
//using PTH.Overworld;
using Obvious.Soap;
using System.Collections.Generic;

namespace KenRampage.Addons.NodeMap
{
    /// <summary>
    /// Manages node-based navigation and interaction in the overworld map system. Handles node selection, 
    /// path highlighting, agent movement between nodes, and broadcasts events for node interactions. 
    /// Integrates with the location system to provide travel costs and progress tracking.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/Node Map/Node Manager")]
    public class NodeManager : MonoBehaviour
    {
        #region Singleton
        public static NodeManager Instance { get; private set; }
        #endregion

        #region Nested Classes
        [System.Serializable]
        public class ReferencesClass
        {
            [Header("Objects")]
            [SerializeField] internal HoveredComponentFinder_Node hoveredNodeFinder;
            [SerializeField] internal Agent defaultAgent;
            [SerializeField] internal Map nodeMap;
            [SerializeField] internal Node defaultNode;
            [Header("Data Input")]
            [SerializeField] internal StringVariable LastNodeNameVariable;
            [Header("Data Output")]
            [SerializeField] internal BoolVariable HeroIsTravelingVariable;
            [SerializeField] internal FloatVariable HoveredNodeRouteCostVariable;
            [SerializeField] internal FloatVariable CurrentPathCostVariable;
            [SerializeField] internal FloatVariable PathProgressVariable;
            [SerializeField] internal FloatVariable RouteCostSumVariable;
            [SerializeField] internal FloatVariable RouteProgressVariable;
            [Header("Formatted Strings Output")]
            [SerializeField] internal StringVariable CurrentNodeNameVariable;
            [SerializeField] internal StringVariable TargetNodeNameVariable;
            [Header("Events Output")]
            [SerializeField] internal ScriptableEventNoParam OnHeroStartedTravelEvent;
            [SerializeField] internal ScriptableEventNoParam OnHeroArriveAtNextNodeEvent;
            [SerializeField] internal ScriptableEventNoParam OnHeroArriveTargetNodeEvent;

        }

        [System.Serializable]
        public class EventsClass
        {
            public UnityEvent<Node> OnNodeHovered;
            public UnityEvent<Node> OnNodeUnhovered;
            public UnityEvent<Node> OnNodeClicked;
            public UnityEvent<Node> OnAgentArrived;
            public UnityEvent<Node> OnAgentArrivedAtTargetNode;
            public UnityEvent<Node> OnAgentDeparted;
        }


        #endregion

        #region Serialized Fields
        [SerializeField] private ReferencesClass _references;
        public EventsClass Events;


        #endregion

        #region Private Fields
        private enum NodeInteractionState
        {
            Unhovered,
            Hovered,
            Selected
        }

        private NodeInteractionState _currentNodeState = NodeInteractionState.Unhovered;
        private float _initialTravelCost;
        #endregion

        #region Public Properties

        public Node CurrentHoveredNode { get; private set; }
        public Node CurrentSelectedNode { get; private set; }
        public Node CurrentAgentNode { get; private set; }

        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            if (_references.defaultAgent != null)
            {
                _references.defaultAgent.OnNodeArrive += HandleAgentArrived;
                _references.defaultAgent.OnMoveStart += HandleAgentDeparted;

                if (_references.LastNodeNameVariable.Value == string.Empty && _references.defaultNode != null)
                {
                    _references.defaultAgent.JumpToNode(_references.defaultNode);
                }
                else
                {
                    Node node = _references.nodeMap.GetNodeByName(_references.LastNodeNameVariable.Value);
                    _references.defaultAgent.JumpToNode(node);
                }

                CurrentAgentNode = _references.defaultAgent.currentNode;
                _references.CurrentNodeNameVariable.Value = CurrentAgentNode.nodeName;


            }
        }

        private void Update()
        {
            HandleHoveredNode();
            if (IsAgentTraveling())
            {
                UpdateCurrentPathInfo();
            }
        }

        private void OnDestroy()
        {
            if (_references.defaultAgent != null)
            {
                _references.defaultAgent.OnNodeArrive -= HandleAgentArrived;
                _references.defaultAgent.OnMoveStart -= HandleAgentDeparted;
            }
        }
        #endregion

        #region Node Interaction Methods
        private void HandleHoveredNode()
        {
            Node hoveredNode = _references.hoveredNodeFinder.FoundComponent;

            if (hoveredNode != CurrentHoveredNode)
            {
                ClearAllHighlightedPaths();

                if (_currentNodeState == NodeInteractionState.Hovered)
                {
                    Events.OnNodeUnhovered.Invoke(CurrentHoveredNode);

                    _currentNodeState = NodeInteractionState.Unhovered;
                }

                CurrentHoveredNode = hoveredNode;

                if (CurrentHoveredNode != null)
                {
                    Events.OnNodeHovered.Invoke(CurrentHoveredNode);

                    _currentNodeState = NodeInteractionState.Hovered;

                    _references.TargetNodeNameVariable.Value = CurrentHoveredNode.nodeName;
                    UpdateHoveredNodeRouteCost();
                    HighlightRouteToHoveredNode();
                }
                else
                {
                    _references.TargetNodeNameVariable.Value = "";
                    UpdateHoveredNodeRouteCost();
                }
            }
        }

        public void HandleClick()
        {
            if (CurrentHoveredNode != null && _currentNodeState == NodeInteractionState.Hovered)
            {
                CurrentSelectedNode = CurrentHoveredNode;
                Events.OnNodeClicked.Invoke(CurrentSelectedNode);

                _currentNodeState = NodeInteractionState.Selected;
            }
        }

        public void DeselectAll()
        {
            if (CurrentSelectedNode != null)
            {
                CurrentSelectedNode = null;
                Events.OnNodeUnhovered.Invoke(CurrentSelectedNode);

                _currentNodeState = NodeInteractionState.Unhovered;
            }
        }
        #endregion

        #region Path and Travel Methods
        private void UpdateHoveredNodeRouteCost()
        {
            if (IsAgentTraveling())
            {
                _references.HoveredNodeRouteCostVariable.Value = _references.RouteCostSumVariable.Value;
            }
            else if (_references.defaultAgent != null && CurrentHoveredNode != null)
            {
                _references.HoveredNodeRouteCostVariable.Value = CalculatePathCost(_references.defaultAgent.currentNode, CurrentHoveredNode);
            }
            else
            {
                _references.HoveredNodeRouteCostVariable.Value = 0f;
            }
        }

        private void UpdateCurrentPathInfo()
        {
            if (_references.defaultAgent != null && _references.defaultAgent.onPath != null)
            {
                _references.CurrentPathCostVariable.Value = _references.defaultAgent.onPath.fromFactor;
                _references.PathProgressVariable.Value = _references.defaultAgent.percentTraveled;

                float totalDistance = _initialTravelCost;
                float distanceTraveled = _initialTravelCost - CalculatePathCost(_references.defaultAgent.currentNode, _references.defaultAgent.targetNode);
                distanceTraveled += _references.defaultAgent.onPath.Distance * _references.defaultAgent.percentTraveled;
                _references.RouteProgressVariable.Value = Mathf.Clamp01(distanceTraveled / totalDistance);
            }
            else
            {
                _references.CurrentPathCostVariable.Value = 0f;
                _references.PathProgressVariable.Value = 0f;
                _references.RouteProgressVariable.Value = 0f;
            }
        }

        private void HighlightRouteToHoveredNode()
        {
            if (_references.defaultAgent != null && CurrentHoveredNode != null)
            {
                List<Node> path = Pathfinding.FindRoute(_references.defaultAgent.currentNode, CurrentHoveredNode, _references.defaultAgent);
                _references.nodeMap.HighlightRoute(path);
            }
            else
            {
                _references.nodeMap.ClearRouteHighlight();
            }
        }

        private void HighlightRouteToTargetNode()
        {
            if (_references.defaultAgent != null && _references.defaultAgent.targetNode != null)
            {
                List<Node> path = Pathfinding.FindRoute(_references.defaultAgent.currentNode, _references.defaultAgent.targetNode, _references.defaultAgent);
                _references.nodeMap.HighlightRoute(path);
            }
        }

        public void TravelToSelectedNode()
        {
            if (_references.nodeMap != null && _references.nodeMap.mapAgents.Count > 0 && CurrentSelectedNode != null)
            {
                _references.HeroIsTravelingVariable.Value = true;

                Agent defaultAgent = _references.nodeMap.mapAgents[0];
                defaultAgent.MoveToTarget(CurrentSelectedNode);

                _references.TargetNodeNameVariable.Value = CurrentSelectedNode.nodeName;
                _initialTravelCost = CalculatePathCost(_references.defaultAgent.currentNode, CurrentSelectedNode);
                _references.RouteCostSumVariable.Value = _initialTravelCost;

                if (defaultAgent.nodeRoute.Count > 0)
                {
                    Node nextNode = defaultAgent.nodeRoute.Peek();
                    Path nextPath = Map.FindValidPath(defaultAgent.currentNode, nextNode);
                    _references.CurrentPathCostVariable.Value = nextPath.fromFactor;
                }

                _references.PathProgressVariable.Value = 0f;
                _references.RouteProgressVariable.Value = 0f;

                HighlightRouteToTargetNode();

                _references.OnHeroStartedTravelEvent.Raise();
            }
        }

        private bool IsAgentTraveling()
        {
            bool isMoving = _references.defaultAgent != null && _references.defaultAgent.isMoving;
            _references.HeroIsTravelingVariable.Value = isMoving;
            return isMoving;
        }

        private void ClearAllHighlightedPaths()
        {
            _references.nodeMap.ClearRouteHighlight();
        }
        #endregion

        #region Agent Handling Methods
        private void HandleAgentArrived(Node node, bool isTarget)
        {
            _references.PathProgressVariable.Value = 1f;

            CurrentAgentNode = node;
            Events.OnAgentArrived.Invoke(CurrentAgentNode);

            _references.CurrentNodeNameVariable.Value = CurrentAgentNode.nodeName;

            _references.OnHeroArriveAtNextNodeEvent.Raise();

            if (isTarget)
            {
                Events.OnAgentArrivedAtTargetNode.Invoke(node);
                _references.TargetNodeNameVariable.Value = "";
                _references.RouteCostSumVariable.Value = 0f;
                _references.CurrentPathCostVariable.Value = 0f;
                _references.PathProgressVariable.Value = 0f;
                _references.RouteProgressVariable.Value = 0f;
                _references.nodeMap.ClearRouteHighlight();
                _references.HeroIsTravelingVariable.Value = false;
                _references.OnHeroArriveTargetNodeEvent.Raise();
            }
            else
            {
                HighlightRouteToTargetNode();
                _references.PathProgressVariable.Value = 0f;
            }

        }

        private void HandleAgentDeparted(Node node)
        {
            Events.OnAgentDeparted.Invoke(node);

            _references.OnHeroStartedTravelEvent.Raise();
        }

        public void JumpAgentToClosestNode()
        {
            if (_references.defaultAgent != null && _references.nodeMap != null)
            {
                Node closestNode = GetClosestNodeToAgent();
                if (closestNode != null)
                {
                    _references.defaultAgent.JumpToNode(closestNode);
                    CurrentAgentNode = closestNode;
                    Events.OnAgentArrived.Invoke(CurrentAgentNode);

                    _references.CurrentNodeNameVariable.Value = CurrentAgentNode.nodeName;
                }
            }
        }

        private Node GetClosestNodeToAgent()
        {
            if (_references.nodeMap == null || _references.defaultAgent == null)
                return null;

            Node closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (Node node in _references.nodeMap.mapNodes)
            {
                float distance = Vector3.Distance(_references.defaultAgent.transform.position, node.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }

            return closestNode;
        }
        #endregion

        #region Utility Methods
        private float CalculatePathCost(Node startNode, Node endNode)
        {
            List<Node> path = Pathfinding.FindRoute(startNode, endNode, _references.defaultAgent);
            float costSum = 0f;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Path pathBetweenNodes = Map.FindValidPath(path[i], path[i + 1]);
                if (pathBetweenNodes != null)
                {
                    costSum += (pathBetweenNodes.fromNode == path[i]) ? pathBetweenNodes.fromFactor : pathBetweenNodes.toFactor;
                }
            }

            return costSum;
        }
        #endregion
    }
}

