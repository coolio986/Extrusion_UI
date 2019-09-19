using Digital_Indicator.Infrastructure.UI.ControlBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Indicator.Infrastructure.UI.Controls
{
    public class EnumItemsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public EnumItem SelectedItem { get; private set; }

        /// <summary>
        /// Gets or sets the previous selected enum item
        /// </summary>
        public EnumItem PreviousSelectedItem { get; private set; }


        private ObservableCollection<EnumItem> _enumList;
        /// <summary>
        /// Gets or sets the EnumList Collection
        /// </summary>
        public ObservableCollection<EnumItem> EnumList
        {
            get { return _enumList; }
            set { _enumList = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets bool for SelectionChanged event masking
        /// </summary>
        public bool StartSelected { get; set; }
        private int _itemIndex;
        /// <summary>
        /// Gets or sets the item index used for the view
        /// </summary>
        public int ItemIndex
        {
            get
            {
                return _itemIndex;
            }
            set
            {
                _itemIndex = value;
                SelectedItem = (EnumList?.Count > 0 && _itemIndex > -1) ? EnumList[value] : new EnumItem();
                base.Value = SelectedItem.ItemValueID;
                if (!StartSelected)
                {
                    SelectionChanged?.Invoke(this, new EventArgs());
                    PreviousSelectedItem = SelectedItem;
                }

                RaisePropertyChanged();
            }
        }

        public event EventHandler SelectionChanged;
        
    }
}
