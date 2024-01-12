namespace CobbleGames.PathFinding
{
    internal struct CGPathFindingNodeData
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public int Index { get; set; }
        
        public float WalkingCostModifier { get; set; }

        private float _WalkingCost;
        public float WalkingCost
        {
            get => _WalkingCost;
            set => _WalkingCost = value + WalkingCostModifier;
        }
        
        public float DistanceToTarget { get; set; }
        public float TotalCost => WalkingCost + DistanceToTarget;
        
        public bool IsWalkable { get; set; }

        public int ParentNodeIndex { get; set; }
    }
}