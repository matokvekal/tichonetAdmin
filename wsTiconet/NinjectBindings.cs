namespace wsTiconet
{
    public class NinjectBindings : Ninject.Modules.NinjectModule  
    {
        public override void Load()
        {
            Bind<ITiconetService>().To<TiconetService>();
        }
    }
}
