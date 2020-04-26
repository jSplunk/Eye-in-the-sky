using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Implementation based on Sebastian Lague
 *  'https://github.com/SebLague/Pathfinding'
 *
 */

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}

public class Heap<T> where T : IHeapItem<T> {

    T[] m_items;
    int m_maxSize;

    public Heap(int _maxSize)
    {
        Count = 0;
        m_maxSize = _maxSize;
        m_items = new T[_maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = Count;
        m_items[Count] = item;
        SortUp(item);
        ++Count;
    }

    public T Pop()
    {
        T firstItem = m_items[0];
        --Count;
        m_items[0] = m_items[Count];
        m_items[0].HeapIndex = 0;
        SortDown(m_items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count { get; private set; }

    public bool Contains(T item)
    {
        return Equals(m_items[item.HeapIndex], item);
    }

    public void Clear()
    {
        Count = 0;
        m_items = new T[m_maxSize];
    }

    void SortDown(T item)
    {
        while(true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;

            int swapIndex = 0;

            if (childIndexLeft < Count)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < Count)
                {
                    if (m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(m_items[swapIndex]) < 0)
                {
                    Swap(item, m_items[swapIndex]);
                }
                else return;
            }
            else return;
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while(true)
        {
            T parentItem = m_items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else break;
        }
    }

    void Swap(T a, T b)
    {
        int bHeapIndex = b.HeapIndex;
        int aHeapIndex = a.HeapIndex;
        m_items[aHeapIndex] = b;
        m_items[bHeapIndex] = a;
        int tmp = aHeapIndex;
        a.HeapIndex = bHeapIndex;
        b.HeapIndex = tmp;
    }
 
}

