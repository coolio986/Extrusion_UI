using Microsoft.Practices.Unity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtrusionUI.Toolkits.PrismExtensions
{
    public class DependentViewRegionBehavior : RegionBehavior
    {

        public const string BehaviorKey = "DependentViewRegionBehavior";

        private IUnityContainer container;

        /// <summary>
        /// Dependencies of the views. For each view, which has
        /// dependent views, stores a list of those dependent views,
        /// so they can be removed when the main view is un-navigated.
        /// </summary>
        private Dictionary<object, List<DependentView>> viewDependencies;

        public DependentViewRegionBehavior(IUnityContainer container)
        {
            this.container = container;
            viewDependencies = new Dictionary<object, List<DependentView>>();
        }

        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var view in e.NewItems)
                    {
                        var dependentViewAttrs = GetCustomAttributes<DependentViewAttribute>(view.GetType());

                        if (dependentViewAttrs.Any())
                        {
                            var dependentViews = new List<DependentView>();

                            foreach (var dependentViewAttr in dependentViewAttrs)
                            {
                                string targetRegion = dependentViewAttr.RegionName;
                                Type targetViewType = dependentViewAttr.ViewType;

                                var targetView = container.Resolve(targetViewType);

                                // set the view model, if needed
                                if (dependentViewAttr.ShareViewModel)
                                {
                                    if (!(view is FrameworkElement) || !(targetView is FrameworkElement))
                                        throw new Exception("Cannot share view model between non framework elements");

                                    ((FrameworkElement)targetView).DataContext =
                                        ((FrameworkElement)view).DataContext;
                                }

                                Region.RegionManager.Regions[targetRegion].Add(targetView);

                                // add newly created, dependent view to the list
                                dependentViews.Add(new DependentView()
                                {
                                    View = targetView,
                                    RegionName = targetRegion,
                                });
                            }

                            // store created views, so we can remove them later
                            viewDependencies[view] = dependentViews;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var view in e.OldItems)
                    {
                        List<DependentView> dependentViews;
                        if (viewDependencies.TryGetValue(view, out dependentViews))
                        {
                            foreach (var v in dependentViews)
                                Region.RegionManager.Regions[v.RegionName].Remove(v.View);
                        }
                    }
                    break;
            }
        }

        static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}

