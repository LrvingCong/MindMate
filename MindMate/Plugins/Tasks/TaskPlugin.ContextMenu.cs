﻿using MindMate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MindMate.Plugins.Tasks
{
    public partial class TaskPlugin : IPluginMapNodeContextMenu
    {
        private MenuItem[] menuItems;

        private MenuItem SetDueDateMenu { get { return menuItems[0]; } }
        private MenuItem UpdateDueDateMenu { get { return menuItems[1]; } }
        private MenuItem CompleteTaskMenu { get { return menuItems[3]; } }

        
        public MenuItem[] CreateContextMenuItemsForNode()
        {
            var t1 = new MenuItem("Set Due Date ...", TaskRes.date_add, SetDueDate_Click);

            var t2 = new MenuItem("Update Due Date ...", TaskRes.date_edit, SetDueDate_Click);

            var t3 = new MenuItem("Quick Due Dates");

            t3.AddDropDownItem(new MenuItem("Today"));
            t3.AddDropDownItem(new MenuItem("Tomorrow"));
            t3.AddDropDownItem(new MenuItem("This Week"));
            t3.AddDropDownItem(new MenuItem("Next Week"));
            t3.AddDropDownItem(new MenuItem("This Month"));
            t3.AddDropDownItem(new MenuItem("Next Month"));
            t3.AddDropDownItem(new MenuItem("No Date"));

            var t4 = new MenuItem("Complete Task", TaskRes.tick, CompleteTask_Click);

            menuItems = new MenuItem[] 
            {
                t1,
                t2,
                t3,
                t4
            };

            return menuItems;
        }

        public void OnContextMenuOpening(SelectedNodes nodes)
        {
            if (DueDateAttribute.Exists(nodes.First))
            {
                SetDueDateMenu.Visible = false;
                UpdateDueDateMenu.Visible = true;
            }
            else
            {
                SetDueDateMenu.Visible = true;
                UpdateDueDateMenu.Visible = false;
            }
        }


        /// <summary>
        /// Should only update the model, all interested views should be updated through the event generated by the model.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="nodes"></param>
        private void SetDueDate_Click(MenuItem menu, SelectedNodes nodes)
        {
            // initialize date time picker
            MapNode.Attribute att;
            if (DueDateAttribute.GetAttribute(nodes.First, out att))
            {
                dateTimePicker.Value = DateHelper.ToDateTime(att.value);
            }
            else
            {
                dateTimePicker.Value = DateTime.Today.Date.AddHours(7);
            }
            
            // show and set due dates
            if (dateTimePicker.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    MapNode node = nodes[i];

                    DueDateAttribute.SetValue(node, DateHelper.ToString(dateTimePicker.Value));
                    CompletionDateAttrbute.Delete(node);

                }
            }
        }

        /// <summary>
        /// Should only update the model, all interested views should be updated through the event generated by the model.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="nodes"></param>
        private void CompleteTask_Click(MenuItem menu, SelectedNodes nodes)
        {
            for(int i =0; i < nodes.Count; i++)
            {
                MapNode node = nodes[i];

                CompleteTask(node);
            }
        }

    }
}
