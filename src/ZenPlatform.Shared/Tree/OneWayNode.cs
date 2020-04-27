using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZenPlatform.Shared.Tree
{
    public class OneWayNode
    {
        private List<OneWayNode> _children;

        public OneWayNode()
        {
            _children = new List<OneWayNode>();
        }

        public IReadOnlyList<OneWayNode> Children => _children;


        public void AddChild(OneWayNode node)
        {
            _children.Add(node);
            if (DetectCircleDependency()) throw new Exception("The circle dependency is not allowed");
        }

        public void RemoveChild(OneWayNode node)
        {
            _children.Remove(node);
        }

        public void Attach(int index, OneWayNode node)
        {
            _children.Insert(index, node);

            if (DetectCircleDependency()) throw new Exception("The circle dependency is not allowed");
        }

        public IEnumerable<OneWayNode> Find(Func<OneWayNode, bool> criteria)
        {
            foreach (var child in _children)
            {
                if (criteria(child)) yield return child;
            }
        }

        public IEnumerable<T> Find<T>(Func<OneWayNode, bool> criteria) where T : OneWayNode
        {
            foreach (var child in _children)
            {
                if (criteria(child) && child is T res) yield return res;

                if (child != null)
                    foreach (var item in child.Find<T>(criteria))
                    {
                        yield return item;
                    }
            }
        }

        public IEnumerable<T> Find<T>() where T : OneWayNode
        {
            return Find<T>((x) => true);
        }

        private bool DetectCircleDependency(OneWayNode detectionPoint = null)
        {
            foreach (var child in Children)
            {
                if (child == null) return false;

                if (child == detectionPoint) return true;
                if (child.DetectCircleDependency(detectionPoint ?? this)) return true;
            }

            return false;
        }

        /// <summary>
        /// Получить дочерний элемент по типу
        /// </summary>
        /// <typeparam name="T">Тип дочернего элемента</typeparam>
        /// <returns></returns>
        public virtual OneWayNode GetChild<T>() where T : OneWayNode
        {
            foreach (var child in Children)
            {
                if (child is T) return child;
            }

            return null;
        }
    }
}