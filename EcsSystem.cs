using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEcs
{
    public interface IEcsSystem
    {

    }

    public interface IEcsInitSystem : IEcsSystem
    {
        void InitSystem();
        void Destroy();
    }

    public interface IEcsRunSystem : IEcsSystem
    {
        void Run();
    }

    public sealed class EcsSystem
    {
        private EcsWorld _world;
        private int _initSystemCount;
        private IEcsInitSystem[] _initSystems = new IEcsInitSystem[16];
        public int _runSystemCount;
        private IEcsRunSystem[] _runSystems = new IEcsRunSystem[16];
        private bool _inited;

        public EcsSystem(EcsWorld world)
        {
            _world = world;
        }

        public int GetInitSystems(ref IEcsInitSystem[] list)
        {
            if (list == null || list.Length < _initSystemCount)
            {
                list = new IEcsInitSystem[_initSystemCount];
            }

            Array.Copy(_initSystems, 0, list, 0, _initSystemCount);
            return _initSystemCount;
        }

        public EcsSystem Add(IEcsSystem system)
        {
            var initSystem = system as IEcsInitSystem;
            if (initSystem != null)
            {
                if (_initSystemCount == _initSystems.Length)
                {
                    Array.Resize(ref _initSystems, _initSystemCount << 1);
                }
                _initSystems[_initSystemCount++] = initSystem;
            }


            var runSystem = system as IEcsRunSystem;
            if (runSystem != null)
            {
                if (_runSystemCount == _runSystems.Length)
                {
                    Array.Resize(ref _runSystems, _runSystemCount << 1);
                }
                _runSystems[_runSystemCount++] = runSystem;
            }

            return this;
        }

        public void Initialize()
        {
            if (_inited)
            {
                throw new Exception(" Group has already initialized");
            }

            for (var i = 0; i < _initSystemCount; i++)
            {
                _initSystems[i].InitSystem();
                _world.ProcessDelayedUpdates();
            }
        }

        public void Destroy(){
            if (!_inited)
            {
                throw new Exception("Group has not initialized");
            }
            for (var i = _initSystemCount - 1; i >= 0; i--)
            {
                _initSystems[i].Destroy();
                _initSystems[i] = null;
            }

            _initSystemCount = 0;

            for (var i = _runSystemCount - 1; i >= 0; i--)
            {
                _runSystems[i] = null;
            }

            _runSystemCount = 0;
        }


        public void Run()
        {
            if (!_inited)
            {
                throw new Exception("Grounp has not initialized");
            }

            for(var i = 0; i < _runSystemCount; i++)
            {
                _runSystems[i].Run();
            }
        }

    }
}
