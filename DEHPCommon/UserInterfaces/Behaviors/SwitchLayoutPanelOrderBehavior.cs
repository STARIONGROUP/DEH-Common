// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchLayoutPanelOrderBehavior.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.UserInterfaces.Behaviors
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using DEHPCommon.Enumerators;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;

    using DevExpress.Mvvm.UI.Interactivity;
    using DevExpress.Xpf.Core.Native;
    using DevExpress.Xpf.Docking;

    /// <summary>
    /// Behavior for DST main windows that allows to switch places between the hub layout panel and the dst layout panel,
    /// <remarks>The behavior well function relies on the panels to be of type <see cref="LayoutGroup"/> and on their name.
    /// Those needs to match the <see cref="LayoutGroupName"/>, <see cref="DstPanelName"/> and <see cref="HubPanelName"/> </remarks>
    /// </summary>
    public class SwitchLayoutPanelOrderBehavior : Behavior<Window>, ISwitchLayoutPanelOrderBehavior
    {
        /// <summary>
        /// Gets the <see cref="LayoutPanel"/> container name
        /// </summary>
        public const string LayoutGroupName = "LayoutGroup";

        /// <summary>
        /// Gets the DST <see cref="LayoutPanel"/>  name
        /// </summary>
        public const string DstPanelName = "DstPanel";

        /// <summary>
        /// Gets the HUB <see cref="LayoutPanel"/>  name
        /// </summary>
        public const string HubPanelName = "HubPanel";

        /// <summary>
        /// The Main <see cref="LayoutGroup"/> container of the Dst panel and the Hub panel
        /// </summary>
        private LayoutGroup layoutGroup;

        /// <summary>
        /// Gets an assert whether the Hub Panel is the source 
        /// </summary>
        private bool IsHubPanelTheSource;
        
        /// <summary>
        /// Gets the actual <see cref="MappingDirection"/>
        /// </summary>
        public MappingDirection MappingDirection { get; private set; }

        /// <summary>
        /// Occurs when this behavior attaches to a view
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.DataContextChanged += this.DataContextChanged;
        }

        /// <summary>
        /// Occurs when this behavior detaches from its associated view
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.DataContextChanged -= this.DataContextChanged;
        }

        /// <summary>
        /// Occurs when the data context of <see cref="Behavior{T}.AssociatedObject"/> changes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/></param>
        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((ISwitchLayoutPanelOrderViewModel) this.AssociatedObject.DataContext).SwitchPanelBehavior = this;
        }

        /// <summary>
        /// Swithes the place of the two panels 
        /// </summary>
        public void Switch()
        {
            this.layoutGroup ??= (LayoutGroup)this.AssociatedObject.VisualChildren().OfType<FrameworkElement>().SingleOrDefault(x => x.Name == LayoutGroupName);

            var ecosimPanel = this.GetLayoutPanelIndex(DstPanelName);
            this.layoutGroup.Items.Move(ecosimPanel, this.IsHubPanelTheSource ? 0 : 2);

            var hubPanel = this.GetLayoutPanelIndex(HubPanelName);
            this.layoutGroup.Items.Move(hubPanel, this.IsHubPanelTheSource ? 2 : 0);

            this.IsHubPanelTheSource = !this.IsHubPanelTheSource;
            this.MappingDirection = this.IsHubPanelTheSource ? MappingDirection.FromHubToDst : MappingDirection.FromDstToHub;
        }

        /// <summary>
        /// Gets the index of the layout panel named after <paramref name="xName"></paramref>
        /// </summary>
        /// <param name="xName">The xName of the panel</param>
        /// <returns>The index</returns>
        private int GetLayoutPanelIndex(string xName)
        {
            return this.layoutGroup.Items.IndexOf(this.layoutGroup.Items.Single(x => x.Name == xName));
        }
    }
}