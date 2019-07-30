using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Shove
{
    /// <summary>
    /// ControlExt 的摘要说明。
    /// </summary>
    public class ControlExt
    {
        /// <summary>
        /// 填充 DropDownList 控件
        /// </summary>
        public static int FillDropDownList(DropDownList ddl, DataTable dt, string TextFieldName, string ValueFieldName)
        {
            ddl.Items.Clear();

            if ((dt == null) || (dt.Rows.Count < 1))
            {
                return -1;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(dr[TextFieldName].ToString(), dr[ValueFieldName].ToString()));
            }

            if (ddl.Items.Count > 0)
            {
                ddl.SelectedIndex = 0;
            }

            return ddl.Items.Count;
        }

        /// <summary>
        /// 递归填充 DropDownList 控件，用于具有 ParentID 指向 ID 的表
        /// </summary>
        public static int FillDropDownList(DropDownList ddl, DataTable dt, string TextFieldName, string IDFieldName, string ParentIDFieldName, long FirstLevelParentIDValue)
        {
            ddl.Items.Clear();

            if ((dt == null) || (dt.Rows.Count < 1))
            {
                return -1;
            }

            DataRow[] drs = dt.Select(ParentIDFieldName + "=" + FirstLevelParentIDValue);

            foreach (DataRow dr in drs)
            {
                ddl.Items.Add(new ListItem(dr[TextFieldName].ToString(), dr[IDFieldName].ToString()));
                FillDropDownList_Sub(ddl, dt, TextFieldName, IDFieldName, ParentIDFieldName, dr[IDFieldName].ToString(), 1);
            }

            if (ddl.Items.Count > 0)
            {
                ddl.SelectedIndex = 0;
            }

            return ddl.Items.Count;
        }

        /// <summary>
        /// 上面一个方法的递归子方法
        /// </summary>
        private static void FillDropDownList_Sub(DropDownList ddl, DataTable dt, string TextFieldName, string IDFieldName, string ParentIDFieldName, string ParentIDFieldValue, int Level)
        {
            string PreFix = "".PadLeft(Level, (char)12288); // &#160; 是半角

            Level++;

            DataRow[] drs = dt.Select(ParentIDFieldName + "=" + ParentIDFieldValue);

            foreach (DataRow dr in drs)
            {
                ddl.Items.Add(new ListItem(PreFix + dr[TextFieldName].ToString(), dr[IDFieldName].ToString()));
                FillDropDownList_Sub(ddl, dt, TextFieldName, IDFieldName, ParentIDFieldName, dr[IDFieldName].ToString(), Level);
            }
        }

        /// <summary>
        /// 设置下拉文本，return -1：下拉无项目。-2：下拉无此项目
        /// </summary>
        public static int SetDownListBoxText(DropDownList ddl, string Text)
        {
            if (ddl.Items.Count == 0)
                return -1;
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == Text)
                {
                    ddl.SelectedIndex = i;
                    return i;
                }
            }
            return -2;
        }

        /// <summary>
        /// 根据Item的Value设置下拉文本，return -1：下拉无项目。-2：下拉无此项目
        /// </summary>
        public static int SetDownListBoxTextFromValue(DropDownList ddl, string Value)
        {
            if (ddl.Items.Count == 0)
                return -1;
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Value == Value)
                {
                    ddl.SelectedIndex = i;
                    return i;
                }
            }
            return -2;
        }

        /// <summary>
        /// 填充 ListBox 控件
        /// </summary>
        public static int FillListBox(ListBox lb, DataTable dt, string TextField, string ValueField)
        {
            lb.Items.Clear();

            if ((dt == null) || (dt.Rows.Count < 1))
            {
                return -1;
            }

            foreach (DataRow dr in dt.Rows)
            {
                lb.Items.Add(new ListItem(dr[TextField].ToString(), dr[ValueField].ToString()));
            }

            if (lb.Items.Count > 0)
            {
                lb.SelectedIndex = 0;
            }

            return lb.Items.Count;
        }

        /// <summary>
        /// 设置ListBox，return -1：无项目。-2：无此项目
        /// </summary>
        public static int SetListBoxText(ListBox lb, string Text)
        {
            if (lb.Items.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < lb.Items.Count; i++)
            {
                if (lb.Items[i].Text == Text)
                {
                    lb.SelectedIndex = i;

                    return i;
                }
            }
            return -2;
        }

        /// <summary>
        /// 根据Item的Value设置 ListBox 文本，return -1：下拉无项目。-2：下拉无此项目
        /// </summary>
        public static int SetListBoxTextFromValue(ListBox lb, string Value)
        {
            if (lb.Items.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < lb.Items.Count; i++)
            {
                if (lb.Items[i].Value == Value)
                {
                    lb.SelectedIndex = i;

                    return i;
                }
            }

            return -2;
        }

        /// <summary>
        /// 根据值设置 CheryTreeView 的选中节点
        /// </summary>
        public static TreeNode SetTreeViewSelected(TreeView tv, string Text)
        {
            if ((tv == null) || (tv.Nodes.Count < 1))
            {
                return null;
            }

            foreach (TreeNode tn in tv.Nodes)
            {
                if (tn.Text == Text)
                {
                    tn.Selected = true;

                    return tn;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据值设置 CheryTreeView 的选中节点
        /// </summary>
        public static TreeNode SetTreeViewSelectedFromValue(TreeView tv, string Value)
        {
            if ((tv == null) || (tv.Nodes.Count < 1))
            {
                return null;
            }

            foreach (TreeNode tn in tv.Nodes)
            {
                if (tn.Value == Value)
                {
                    tn.Selected = true;

                    return tn;
                }
            }

            return null;
        }

        /// <summary>
        /// WinForm 中 ListItem, DropdownList 的 Item，可以记录 value
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 
            /// </summary>
            public string Text;
            /// <summary>
            /// 
            /// </summary>
            public object Value;

            /// <summary>
            /// 
            /// </summary>
            public Item()
            {
                Text = "";
                Value = null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            public Item(string text)
            {
                Text = text;
                Value = null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            /// <param name="value"></param>
            public Item(string text, object value)
            {
                Text = text;
                Value = value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Text;
            }
        }
    }
}
