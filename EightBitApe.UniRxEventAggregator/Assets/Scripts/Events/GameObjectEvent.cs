﻿using UnityEngine;

namespace Events
{
    public class GameObjectEvent : IGameObjectEvent
    {
        public GameObject GameObject { get; set; }

        public GameObjectEvent(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
}
