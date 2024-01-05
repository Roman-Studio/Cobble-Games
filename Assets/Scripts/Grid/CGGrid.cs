using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CobbleGames.Grid
{
    [System.Serializable]
    public class CGGrid<TElementType> 
        where TElementType : ICGGridElement
    {
        [field: SerializeField, ReadOnly]
        public int SizeX { get; private set; }
        
        [field: SerializeField, ReadOnly]
        public int SizeY { get; private set; }

        [field: SerializeField, ReadOnly] 
        private TElementType[] _GridElements;

        public CGGrid(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            _GridElements = new TElementType[SizeX * SizeY];
        }

        public bool TryGetElement(int x, int y, out TElementType foundElement)
        {
            foundElement = default;
            
            if (x < 0 || x >= SizeX)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= SizeY)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            var targetIndex = SizeX * y + x;

            if (targetIndex < 0)
            {
                Debug.LogError($"[{nameof(CGGrid<TElementType>)}.{nameof(TryGetElement)}] Resulting element index: ({targetIndex}) is negative!");
                return false;
            }

            if (targetIndex >= _GridElements.Length)
            {
                Debug.LogError($"[{nameof(CGGrid<TElementType>)}.{nameof(TryGetElement)}] Resulting element index ({targetIndex}) is grater than the number of stored elements!");
                return false;
            }

            foundElement = _GridElements[targetIndex];
            return true;
        }

        public bool TrySetElement(int x, int y, TElementType newElement)
        {
            if (x < 0 || x >= SizeX)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= SizeY)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            var targetIndex = SizeX * y + x;

            if (targetIndex < 0)
            {
                Debug.LogError($"[{nameof(CGGrid<TElementType>)}.{nameof(TryGetElement)}] Resulting element index: ({targetIndex}) is negative!");
                return false;
            }

            var maxIndex = SizeX * SizeY;

            if (targetIndex >= maxIndex)
            {
                Debug.LogError($"[{nameof(CGGrid<TElementType>)}.{nameof(TryGetElement)}] Resulting element index ({targetIndex}) is grater than the number of stored elements ({maxIndex})!");
                return false;
            }

            newElement.X = x;
            newElement.Y = y;
            _GridElements[targetIndex] = newElement;
            return true;
        }

        public void ForEach(Action<TElementType> func)
        {
            for (var x = 0; x < SizeX; x++)
            {
                for (var y = 0; y < SizeY; y++)
                {
                    func.Invoke(_GridElements[SizeX * y + x]);
                }
            }
        }

        public void ForEach(Action<int, int, TElementType> func)
        {
            for (var x = 0; x < SizeX; x++)
            {
                for (var y = 0; y < SizeY; y++)
                {
                    func.Invoke(x, y, _GridElements[SizeX * y + x]);
                }
            }
        }
    }
}