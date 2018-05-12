
namespace AnimalFarm.Model.Tests.Builders
{
    public abstract class NestedBuilder<TTarget, TParent>
    {
        protected readonly TTarget _target;
        protected readonly TParent _parent;

        public NestedBuilder(TTarget target, TParent parent)
        {
            _target = target;
            _parent = parent;
        }

        public TParent And { get => _parent; }
    }
}
