namespace shop.UIForms.Infrastructure
{
    using shop.UIForms.ViewModels;


    public class InstanceLocator
    {
        public MainViewModel Main { get; set; }
        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        }
    }
}
