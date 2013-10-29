using System;

namespace IdaNet
{
    public interface IIdaPlugin
    {
        int Initialize();
        void Run();
        void Terminate();
    }
}
