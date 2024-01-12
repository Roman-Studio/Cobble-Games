namespace CobbleGames.PathFinding
{
    internal struct CGPathFindingNodeData
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public int Index { get; set; }
        
        public float GCostModifier { get; set; }

        private float _GCost;
        public float GCost
        {
            get => _GCost;
            set => _GCost = value + GCostModifier;
        }
        
        public float HCost { get; set; }
        public float FCost => GCost + HCost;
        
        public bool IsWalkable { get; set; }

        public int ParentNodeIndex { get; set; }
    }
}