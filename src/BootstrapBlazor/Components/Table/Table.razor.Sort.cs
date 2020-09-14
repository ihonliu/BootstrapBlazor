﻿using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    partial class Table<TItem>
    {
        /// <summary>
        /// 获得/设置 排序字段名称
        /// </summary>
        protected string? SortName { get; set; }

        /// <summary>
        /// 获得/设置 排序方式
        /// </summary>
        protected SortOrder SortOrder { get; set; }

        /// <summary>
        /// 获得/设置 升序图标
        /// </summary>
        [Parameter]
        public string SortIconAsc { get; set; } = "fa fa-sort-asc";

        /// <summary>
        /// 获得/设置 降序图标
        /// </summary>
        [Parameter]
        public string SortIconDesc { get; set; } = "fa fa-sort-desc";

        /// <summary>
        /// 获得/设置 默认图标
        /// </summary>
        [Parameter]
        public string SortIcon { get; set; } = "fa fa-sort";

        /// <summary>
        /// 获得/设置 表头排序时回调方法
        /// </summary>
        protected Func<Task> OnSortAsync { get; set; } = () => Task.CompletedTask;

        /// <summary>
        /// 点击列进行排序方法
        /// </summary>
        protected async Task OnClickHeader(ITableColumn col)
        {
            if (col.Sortable)
            {
                if (SortOrder == SortOrder.Unset) SortOrder = SortOrder.Asc;
                else if (SortOrder == SortOrder.Asc) SortOrder = SortOrder.Desc;
                else if (SortOrder == SortOrder.Desc) SortOrder = SortOrder.Unset;
                SortName = col.GetFieldName();

                // 通知 Table 组件刷新数据
                if (OnSortAsync != null) await OnSortAsync.Invoke();
            }
        }

        /// <summary>
        /// 获取指定列头样式字符串
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected string? GetHeaderClassString(ITableColumn col) => CssBuilder.Default()
            .AddClass("sortable", col.Sortable)
            .AddClass("filterable", col.Filterable)
            .AddClass(GetFixedCellClassString(col))
            .Build();

        /// <summary>
        /// 获得指定列头固定列样式
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected string? GetFixedCellClassString(ITableColumn col) => CssBuilder.Default()
            .AddClass("fixed", col.Fixed)
            .AddClass("fixed-right", col.Fixed && IsTail(col))
            .Build();

        private bool IsTail(ITableColumn col)
        {
            var middle = Math.Ceiling(Columns.Count * 1.0 / 2);
            var index = Columns.IndexOf(col);
            return middle < index;
        }

        /// <summary>
        /// 获得指定列头固定列样式
        /// </summary>
        /// <param name="col"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        protected string? GetFixedCellStyleString(ITableColumn col, int margin = 0)
        {
            string? style = null;
            if (col.Fixed)
            {
                var defaultWidth = 200;
                var isTail = IsTail(col);
                var index = Columns.IndexOf(col);
                var width = 0;
                var start = 0;
                if (isTail)
                {
                    // after
                    while (index + 1 < Columns.Count)
                    {
                        width += Columns[index++].Width ?? defaultWidth;
                    }
                    // 如果是固定表头时增加滚动条位置
                    if (Height.HasValue && (index + 1) == Columns.Count) width += margin;
                    style = $"right: {width}px;";
                }
                else
                {
                    while (index > start)
                    {
                        width += Columns[start++].Width ?? defaultWidth;
                    };
                    style = $"left: {width}px;";
                }
            }
            return style;
        }

        /// <summary>
        /// 获取指定列头样式字符串
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected string? GetHeaderWrapperClassString(ITableColumn col) => CssBuilder.Default("table-cell")
            .AddClass("is-sort", col.Sortable)
            .AddClass("is-filter", col.Filterable)
            .Build();

        /// <summary>
        /// 获得 Header 中表头文字样式
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected string? GetHeaderTextClassString(ITableColumn col) => CssBuilder.Default("table-text")
            .AddClass("text-left", col.Align == Alignment.Left)
            .AddClass("text-right", col.Align == Alignment.Right)
            .AddClass("text-center", col.Align == Alignment.Center)
            .Build();

        /// <summary>
        /// 获得 Cell 文字样式
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected string? GetCellClassString(ITableColumn col) => CssBuilder.Default("table-cell")
            .AddClass("justify-content-start", col.Align == Alignment.Left)
            .AddClass("justify-content-end", col.Align == Alignment.Right)
            .AddClass("justify-content-center", col.Align == Alignment.Center)
            .AddClass(col.CssClass)
            .Build();

        /// <summary>
        /// 获取指定列头样式字符串
        /// </summary>
        /// <returns></returns>
        protected string? GetIconClassString(string fieldName)
        {
            var order = SortName == fieldName ? SortOrder : SortOrder.Unset;
            return order switch
            {
                SortOrder.Asc => SortIconAsc,
                SortOrder.Desc => SortIconDesc,
                _ => SortIcon
            };
        }
    }
}
