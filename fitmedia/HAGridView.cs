using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace fitmedia
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:HAGridView runat=server></{0}:HAGridView>")]
    public class HAGridView : GridView, IPageableItemContainer
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(Text);
        }

        /// <summary>
        /// TotalRowCountAvailable event key
        /// </summary>
        private static readonly object EventTotalRowCountAvailable = new object();

        /// <summary>
        /// IPageableItemContainer's MaximumRows  = PageSize property
        /// </summary>
        int IPageableItemContainer.MaximumRows
        {

            get { return this.PageSize; }

        }

        /// <summary>
        /// IPageableItemContainer's StartRowIndex = PageSize * PageIndex properties
        /// </summary>
        int IPageableItemContainer.StartRowIndex
        {
            get { return this.PageSize * this.PageIndex; }
        }

        /// <summary>
        /// Set the control with appropriate parameters and bind to right chunk of data.
        /// </summary>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <param name="databind"></param>
        void IPageableItemContainer.SetPageProperties(int startRowIndex, int maximumRows, bool databind)
        {
            int newPageIndex = (startRowIndex / maximumRows);

            this.PageSize = maximumRows;

            if (this.PageIndex != newPageIndex)
            {
                bool isCanceled = false;
                if (databind)
                {
                    //  create the event arguments and raise the event
                    GridViewPageEventArgs args = new GridViewPageEventArgs(newPageIndex);
                    this.OnPageIndexChanging(args);
                    isCanceled = args.Cancel;
                    newPageIndex = args.NewPageIndex;
                }

                //  if the event wasn't cancelled change the paging values
                if (!isCanceled)
                {
                    this.PageIndex = newPageIndex;
                    if (databind)
                        this.OnPageIndexChanged(EventArgs.Empty);
                }

                if (databind)
                    this.RequiresDataBinding = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<PageEventArgs> IPageableItemContainer.TotalRowCountAvailable
        {
            add { base.Events.AddHandler(HAGridView.EventTotalRowCountAvailable, value); }
            remove { base.Events.RemoveHandler(HAGridView.EventTotalRowCountAvailable, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTotalRowCountAvailable(PageEventArgs e)
        {
            EventHandler<PageEventArgs> handler = (EventHandler<PageEventArgs>)base.Events[HAGridView.EventTotalRowCountAvailable];
            if (handler != null)
            {
                handler(this, e);
            }
        }


        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            int rows = base.CreateChildControls(dataSource, dataBinding);
 
            //  if the paging feature is enabled, determine the total number of rows in the datasource
            if (this.AllowPaging)
            {
                //  if we are databinding, use the number of rows that were created, otherwise cast the datasource to an Collection and use that as the count
                int totalRowCount = dataBinding ? rows : ((ICollection)dataSource).Count;
 
                //  raise the row count available event
                IPageableItemContainer pageableItemContainer = this as IPageableItemContainer;

                this.OnTotalRowCountAvailable(new PageEventArgs(pageableItemContainer.StartRowIndex,pageableItemContainer.MaximumRows,totalRowCount));
 
                //  make sure the top and bottom pager rows are not visible
                if (this.TopPagerRow != null)
                    this.TopPagerRow.Visible = false;
 
                if (this.BottomPagerRow != null)
                    this.BottomPagerRow.Visible = false;
           }

           return rows;
        }
    }
}
