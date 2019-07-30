using System;
using System.Web;
using System.Web.UI;

namespace Shove._Web
{
    /// <summary>
    /// JavaScript 的摘要说明。
    /// </summary>
    public class JavaScript
    {
        #region Alert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Msg">脚本弹出的消息框的内容</param>
        public static void Alert(Page page, string Msg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<Script language=javascript>alert('" + Msg + "');</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Msg">脚本弹出的消息框的内容</param>
        /// <param name="RedirectUrl">按确定关闭对话框后，重定向的地址，如果设置为 null, 将重新加载本页，如果设置一个路径，将转跳到该路径</param>
        public static void Alert(Page page, string Msg, string RedirectUrl)
        {
            Alert(page, Msg, RedirectUrl, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Msg">脚本弹出的消息框的内容</param>
        /// <param name="RedirectUrl">按确定关闭对话框后，重定向的地址，如果设置为 null, 将重新加载本页，如果设置一个路径，将转跳到该路径</param>
        /// <param name="FrameName">如果需要其他框架内发生转跳，则输入该框架名称，如："top", "parent", "content"...</param>
        public static void Alert(Page page, string Msg, string RedirectUrl, string FrameName)
        {
            FrameName = FrameName.Trim();

            if (!String.IsNullOrEmpty(FrameName))
            {
                FrameName += ".";
            }


            if (!String.IsNullOrEmpty(RedirectUrl))
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "", "<Script language=javascript>alert('" + Msg + "');window." + FrameName + "location.replace('" + RedirectUrl + "')</script>");
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "", "<Script language=javascript>alert('" + Msg + "');window." + FrameName + "location.href = window." + FrameName + "location.href</script>");
            }
        }

        #endregion

        #region Window Operate

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public static void ClosePage(Page page)
        {
            page.Response.Write("<script language=javascript>window.opener=null;window.close();</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        public static void OpenModalWindow(Page page, string url)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>window.showModalDialog('" + url + "',null,'center: Yes; help: No; resizable: No; status: No; maximize:No; minimize:no; scrollbars:no');</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void OpenModalWindow(Page page, string url, int width, int height)
        {
            string dialogSize = "dialogHeight:" + height.ToString() + "px; dialogWidth:" + width.ToString() + "px; center: Yes; help: No; resizable: No; status: No; maximize:No; minimize:no; scrollbars:no";
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>window.showModalDialog('" + url + "',null,'" + dialogSize + "');</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public static void OpenModalWindow(Page page, string url, int width, int height, int top, int left)
        {
            string dialogSize = "dialogHeight:" + height.ToString() + "px; dialogLeft:" + left.ToString() + "px; dialogWidth:" + width.ToString() + "px;dialogTop:" + top.ToString() + "px; center: No; help: No; resizable: No; status: No; maximize:no; minimize:no; scrollbars:no";
            page.ClientScript.RegisterStartupScript(page.GetType(), "", "<script language=javascript>window.showModalDialog('" + url + "',null,'" + dialogSize + "');</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public static void PopupWindow(Page page, string url, int width, int height, int top, int left)
        {
            page.Response.Write("<script Language=JavaScript>ChildWindow = window.open('" + url + "', 'popupwindow', 'width=" + width.ToString() + ",height=" + height.ToString() + ",top=" + top.ToString() + ",left=" + left.ToString() + ",toolbar=no,status=no,location=no,menubar=no,directories=no,scrollbars=no,resizable=no')</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        public static void PopupWindow(Page page, string url)
        {
            page.Response.Write("<script Language=JavaScript>ChildWindow = window.open('" + url + "', 'popupwindow', 'toolbar=no,status=no,location=no,menubar=no,directories=no,scrollbars=no,resizable=no')</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        public static void OpenWindow(Page page, string url)
        {
            page.Response.Write("<script Language=JavaScript>ChildWindow = window.open('" + url + "', 'newwindow', '')</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetWindowSize(Page page, int x, int y)
        {
            page.Response.Write("<script language='javascript'>window.resizeTo(" + x.ToString() + "," + y.ToString() + ");</script>");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="FrameName"></param>
        public static void RefreshFrame(Page page, string FrameName)
        {
            page.Response.Write("<script language=javascript>window.parent.frames[\"" + FrameName + "\"].document.location.href=window.parent.frames[\"" + FrameName + "\"].document.location.href;</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        public static void SetHomePage(Page page, string url)
        {
            page.Response.Write("<script language=javascript>this.style.behavior=\'url(#default#homepage)\';this.setHomePage(\'" + url + "\');return false;</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="url"></param>
        /// <param name="SiteName"></param>
        public static void AddFavorite(Page page, string url, string SiteName)
        {
            page.Response.Write("<script language=javascript>window.external.addFavorite(\'" + url + "\',\'" + SiteName + "\');</script>");
        }
    }
}
