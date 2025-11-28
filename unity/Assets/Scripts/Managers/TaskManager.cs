using UnityEngine;
using System.Collections.Generic;
using FiveElements.Shared;
using FiveElements.Unity.Managers;

namespace FiveElements.Unity.Managers
{
    public enum TaskType
    {
        Collect,
        Kill,
        Talk
    }
    
    [System.Serializable]
    public class Task
    {
        public TaskType Type;
        public string Description;
        public bool Completed;
        public object Target; // 目标对象（资源、怪物或NPC）
    }
    
    public class TaskManager : MonoBehaviour
    {
        public Task CurrentTask { get; private set; }
        
        public void GenerateTask()
        {
            TaskType[] taskTypes = { TaskType.Collect, TaskType.Kill, TaskType.Talk };
            TaskType type = taskTypes[Random.Range(0, taskTypes.Length)];
            
            switch (type)
            {
                case TaskType.Collect:
                    GenerateCollectTask();
                    break;
                case TaskType.Kill:
                    GenerateKillTask();
                    break;
                case TaskType.Talk:
                    GenerateTalkTask();
                    break;
            }
        }
        
        private void GenerateCollectTask()
        {
            ElementType[] elements = { ElementType.Metal, ElementType.Wood, ElementType.Water, ElementType.Fire, ElementType.Earth };
            ElementType element = elements[Random.Range(0, elements.Length)];
            int amount = Mathf.FloorToInt(5 + OfflineGameManager.Instance.WorldDistance * 2);
            
            CurrentTask = new Task
            {
                Type = TaskType.Collect,
                Description = "采集一次资源",
                Completed = false
            };
            
            // 生成资源
            OfflineGameManager.Instance.SpawnResource(element, amount);
        }
        
        private void GenerateKillTask()
        {
            int health = 50 + OfflineGameManager.Instance.WorldDistance * 10;
            int damage = 5 + OfflineGameManager.Instance.WorldDistance * 2;
            float speed = 0.5f + OfflineGameManager.Instance.WorldDistance * 0.1f;
            
            CurrentTask = new Task
            {
                Type = TaskType.Kill,
                Description = "击杀一个怪物",
                Completed = false
            };
            
            // 生成怪物
            OfflineGameManager.Instance.SpawnMonster(health, damage, speed);
        }
        
        private void GenerateTalkTask()
        {
            CurrentTask = new Task
            {
                Type = TaskType.Talk,
                Description = "与NPC对话",
                Completed = false
            };
            
            // 生成NPC
            OfflineGameManager.Instance.SpawnNPC("你好，勇敢的冒险者！");
        }
        
        public void CompleteTask()
        {
            if (CurrentTask != null)
            {
                CurrentTask.Completed = true;
            }
        }
    }
}