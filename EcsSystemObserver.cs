using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleEcs
{

    public sealed class EcsEntityObserver:MonoBehaviour
    {

    }

    class EcsSystemObserver : MonoBehaviour, IEcsSystemsDebugListener
    {
        EcsSystem _system;
        public static GameObject Create(EcsSystem ecsSystem, string name = null)
        {
            if(ecsSystem == null)
            {
                throw new ArgumentException("EcsSystem");
            }
            var go = new GameObject(name != null ? string.Format("[EcsSystem{0}]", name) : "[EcsSystem]");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.NotEditable;
            var observer = go.AddComponent<EcsSystemObserver>();
            observer._system = ecsSystem;

            return go;
        }

        public  EcsSystem GetSystem()
        {
            return _system;
        }

        public void OnDestroy()
        {
            if(_system != null)
            {
                _system.Destroy();
                _system = null;
            }
        }
    }

    

}
