using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public abstract class EntityFactory
    {
        public abstract IBase Call(params object[] parms);
    }

    public class EntityFactory<Arg1> : EntityFactory
    {
        public Func<Arg1, IBase> Factory { get; }

        public EntityFactory(Func<Arg1, IBase> factory)
        {
            this.Factory = factory;
        }

        public override IBase Call(params object[] parms)
        {
            return this.Factory((Arg1) parms[0]);
        }
    }

    public class EntityFactory<Arg1, Arg2> : EntityFactory
    {
        public Func<Arg1, Arg2, IBase> Factory { get; }

        public EntityFactory(Func<Arg1, Arg2, IBase> factory)
        {
            this.Factory = factory;
        }

        public override IBase Call(params object[] parms)
        {
            return this.Factory((Arg1)parms[0], (Arg2)parms[1]);
        }
    }

    public class EntityFactory<Arg1, Arg2, Arg3> : EntityFactory
    {
        public Func<Arg1, Arg2, Arg3, IBase> Factory { get; }

        public EntityFactory(Func<Arg1, Arg2, Arg3, IBase> factory)
        {
            this.Factory = factory;
        }

        public override IBase Call(params object[] parms)
        {
            return this.Factory((Arg1)parms[0], (Arg2)parms[1], (Arg3)parms[2]);
        }
    }

    public class EntityFactory<Arg1, Arg2, Arg3, Arg4> : EntityFactory
    {
        public Func<Arg1, Arg2, Arg3, Arg4, IBase> Factory { get; }

        public EntityFactory(Func<Arg1, Arg2, Arg3, Arg4, IBase> factory)
        {
            this.Factory = factory;
        }

        public override IBase Call(params object[] parms)
        {
            return this.Factory((Arg1)parms[0], (Arg2)parms[1], (Arg3)parms[2], (Arg4)parms[3]);
        }
    }

    public class EntityFactory<Arg1, Arg2, Arg3, Arg4, Arg5> : EntityFactory
    {
        public Func<Arg1, Arg2, Arg3, Arg4, Arg5, IBase> Factory { get; }

        public EntityFactory(Func<Arg1, Arg2, Arg3, Arg4, Arg5, IBase> factory)
        {
            this.Factory = factory;
        }

        public override IBase Call(params object[] parms)
        {
            return this.Factory((Arg1)parms[0], (Arg2)parms[1], (Arg3)parms[2], (Arg4)parms[3], (Arg5)parms[4]);
        }
    }
}
