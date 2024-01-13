using System.Collections.Generic;
using UnityEngine;

namespace CobbleGames.PathFinding
{
    [RequireComponent(typeof(LineRenderer))]
    public class CGPathDrawer : MonoBehaviour
    {
        private LineRenderer _LineRenderer;
        public LineRenderer LineRenderer
        {
            get
            {
                if (_LineRenderer != null)
                {
                    return _LineRenderer;
                }

                _LineRenderer = GetComponent<LineRenderer>();
                return _LineRenderer;
            }
        }

        [field: SerializeField]
        protected Vector3 LineAdjustVector { get; private set; } = new (0f, 0.1f, 0f);

        public void UpdateLineColor(Color newColor)
        {
            LineRenderer.startColor = newColor;
            LineRenderer.endColor = newColor;
        }

        public void UpdateLine(IList<ICGPathFindingNode> nodes)
        {
            if (nodes == null)
            {
                LineRenderer.positionCount = 0;
                return;
            }
            
            LineRenderer.positionCount = nodes.Count + 1;
            LineRenderer.SetPosition(0, transform.position);
            
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                LineRenderer.SetPosition(i + 1, node.NodePosition + LineAdjustVector);
            }
        }
    }
}