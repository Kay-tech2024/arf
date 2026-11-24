using ARF.Core;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace ARF.Web.App_Start.Extensions
{
    public enum DateType { Date, Time, DateTime }

    public static class HtmlHelperExtensions
    {
        private const string SCRIPTBLOCK_BUILDER = "ScriptBlockBuilder";

        public static IHtmlString RawActionLink(this HtmlHelper htmlHelper, string linkText, string action, string controller, object routeValues, object htmlAttributes)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var anchor = new TagBuilder("a");
            anchor.InnerHtml = linkText;
            anchor.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            anchor.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(anchor.ToString());
        }

        public static MvcHtmlString DatePickerFor<TModel, TProp>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProp>> expression, DateType type = DateType.DateTime, object htmlAttributesObj = null)
        {
            var @class = (string)null;
            var @readonly = (string)null;
            var disabled = (string)null;

            @class = (string)htmlAttributesObj.GetPropValue("class");

            if (htmlAttributesObj.HasProperty("readonly") && (string)htmlAttributesObj.GetPropValue("readonly") != null)
            {
                @readonly = (string)htmlAttributesObj.GetPropValue("readonly");
            }

            if (htmlAttributesObj.HasProperty("disabled") && (string)htmlAttributesObj.GetPropValue("disabled") != null)
            {
                disabled = (string)htmlAttributesObj.GetPropValue("disabled");
            }
            var valueStr = (string)htmlAttributesObj.GetPropValue("value");
            var placeholder = (string)htmlAttributesObj.GetPropValue("placeholder");

            if (@class != null)
                @class += " " + (type == DateType.DateTime ? "date-time-picker" : (type == DateType.Date ? "date-picker" : "time-picker"));
            else
                @class = type == DateType.DateTime ? "date-time-picker" : (type == DateType.Date ? "date-picker" : "time-picker");

            InputWithClassAndValue htmlAttributes = null;

            if (disabled != null || @readonly != null)
            {
                htmlAttributes = new DisabledInputWithClassAndValue()
                {
                    @class = @class,
                    placeholder = placeholder,
                    value = valueStr,
                    @readonly = @readonly,
                    disabled = disabled
                };
            }
            else
            {
                htmlAttributes = new InputWithClassAndValue()
                {
                    @class = @class,
                    placeholder = placeholder,
                    value = valueStr
                };
            }

            var oModelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = (DateTime?)null;

            if (oModelMetadata.Model is DateTime)
            {
                value = ((DateTime)oModelMetadata.Model);
            }
            else if (oModelMetadata.Model is DateTime?)
            {
                value = ((DateTime?)oModelMetadata.Model);
            }

            if (value != null)
            {
                if (htmlAttributes.value == null)
                {
                    switch (type)
                    {
                        case DateType.DateTime:
                            htmlAttributes.value = Utils.FormatDateTime(value);
                            break;

                        case DateType.Date:
                            htmlAttributes.value = Utils.FormatDate(value);
                            break;

                        case DateType.Time:
                            htmlAttributes.value = Utils.FormatTime(value);
                            break;
                    }
                }

                var result = htmlHelper.TextBoxFor(expression, (object)htmlAttributes);

                return new MvcHtmlString(Regex.Replace(result.ToHtmlString(), "value\\=\"[a-zA-Z0-9/\\s-:]*", "value=\"" + htmlAttributes.value + "\""));
            }
            else
            {
                return htmlHelper.TextBoxFor(expression, (object)htmlAttributes);
            }
        }

        public static MvcHtmlString ScriptBlock(this WebViewPage webPage, Func<dynamic, HelperResult> template)
        {
            if (!webPage.IsAjax)
            {
                var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                scriptBuilder.Append(template(null).ToHtmlString());
                webPage.Context.Items[SCRIPTBLOCK_BUILDER] = scriptBuilder;

                return new MvcHtmlString(string.Empty);
            }

            return new MvcHtmlString(template(null).ToHtmlString());
        }

        public static MvcHtmlString WriteScriptBlocks(this WebViewPage webPage)
        {
            var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new MvcHtmlString(scriptBuilder.ToString());
        }
    }

    public class InputWithClassAndValue
    {
        public string @class { get; set; }

        public string value { get; set; }
        public string placeholder { get; set; }
    }

    public class DisabledInputWithClassAndValue : InputWithClassAndValue
    {
        public string @readonly { get; set; }
        public string disabled { get; set; }
    }
}