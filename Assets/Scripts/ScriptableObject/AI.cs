using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : ScriptableObject
{
    const int STANDARD_MEMORY = 10;

    protected class MemoryObject
    {
        event Action<StatBlock, MemoryObject> onExpiration;
        public readonly StatBlock source;
        float threat;
        float maxMemory;
        float _memory;
        public float threatValue { get { return _memory / maxMemory * threat; } }

        public MemoryObject(ref StatBlock source, float threatValue, float memory, Action<StatBlock, MemoryObject> removeMemory)
        {
            this.source = source;
            threat = threatValue;
            maxMemory = memory;
            _memory = maxMemory;
            onExpiration = removeMemory;
        }

        public void Decay(float decay)
        {
            _memory -= decay;
            if (_memory <= 0)
            {
                onExpiration(source, this);
            }
        }
    }

    [SerializeField] float memorySpan;

    //Instead of an int, I need a list of memory objects that decay over time and adding them all together creates a threat level
    protected Dictionary<StatBlock, List<MemoryObject>> threatDictionary = new Dictionary<StatBlock, List<MemoryObject>>();

    public void AddThreat(ref StatBlock statBlock, int value) //Actually, I think I should let the AI decide how long it should remember things for
    {
        MemoryObject newMemory = new MemoryObject(ref statBlock, value, STANDARD_MEMORY, RemoveMemory); GameManager.instance.onUpdate += newMemory.Decay;
        if (!threatDictionary.ContainsKey(statBlock)) { threatDictionary.Add(statBlock, new List<MemoryObject>() { newMemory }); return; }
        threatDictionary[statBlock].Add(newMemory);
    }

    //I'm still not totally sure there are no references to this memory left, so if the game starts slowing down as battles drag on this is something to look at
    void RemoveMemory(StatBlock source, MemoryObject memory) { threatDictionary[source].Remove(memory); GameManager.instance.onUpdate -= memory.Decay; }

    public virtual void ExecuteAI(StatBlock myStatBlock)
    {
        
    }
}
