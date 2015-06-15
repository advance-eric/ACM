using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public enum Icons
        {
            arrow_n,
            arrow_e,
            arrow_s,
            arrow_w,
            seek_n,
            seek_e,
            seek_s,
            seek_w,
            arrowhead_n,
            arrowhead_e,
            arrowhead_s,
            arrowhead_w,
            expand,
            collapse,
            expand_w,
            collapse_w,
            plus,
            tick,
            close,
            pencil,
            cancel,
            funnel,
            funnel_clear,
            calendar,
            clock,
            search,
            refresh,
            restore,
            maximize,
            minimize,
            custom,
            insert_n,
            insert_m,
            insert_s,
            note,
            folder_add,
            folder_up
        }


        public enum KendoTextBoxType
        {
            Standard,
            Password
        }

        public static MvcHtmlString KendoSubmitButton(this HtmlHelper helper, string text, string name = "", object htmlAttributes = null, string location = "", string jscript = "", bool? isDefault = true)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var builder = new TagBuilder("input");

            if (htmlAttributes != null)
                builder.MergeAttributes(attributes);

            if (string.IsNullOrWhiteSpace(name))
                name = text.Replace(" ", "");

            builder.Attributes.Add("type", "submit");
            builder.Attributes.Add("value", text);
            builder.Attributes.Add("name", name);
            builder.Attributes.Add("id", name);

            if (!string.IsNullOrWhiteSpace(jscript))
            {
                if (!jscript.EndsWith(")"))
                    jscript += "()";

                builder.Attributes.Add("onclick", string.Format("{0};return false;", jscript));
            }
            else if (!string.IsNullOrWhiteSpace(location))
                builder.Attributes.Add("onclick", string.Format("location.href=\"{0}\"", location));
            else
                builder.Attributes.Add("onclick", string.Format("ButtonClick(this,'{0}'); return false;", name));

            if (isDefault == true)
                builder.Attributes.Add("data-default", "true");

            if (builder.Attributes.Where(m => m.Key == "style").Count() == 0)
                builder.Attributes.Add("style", "width: 150px");

            builder.AddCssClass("k-button");
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static string Image(this HtmlHelper helper, string id, string url, string alternateText, object htmlAttributes)
        {
            var builder = new TagBuilder("img");
            builder.GenerateId(id);
            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static MvcHtmlString TextBoxPlaceHolderFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (!String.IsNullOrEmpty(labelText))
            {
                if (htmlAttributes == null)
                {
                    htmlAttributes = new Dictionary<string, object>();
                }
                htmlAttributes.Add("placeholder", labelText);
            }
            return html.TextBoxFor(expression, htmlAttributes);
        }
        public static MvcHtmlString BootstrapPasswordFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            htmlAttributes.Add("class", "form-control");

            if (!String.IsNullOrEmpty(labelText))
                htmlAttributes.Add("placeholder", labelText);

            if (htmlAttributes.Where(m => m.Key == "style").Count() == 0)
                htmlAttributes.Add("style", "width: 250px");

            return html.PasswordFor(expression, htmlAttributes);
        }
        public static MvcHtmlString BootstrapTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            htmlAttributes.Add("class", "form-control");

            if (!String.IsNullOrEmpty(labelText))
                htmlAttributes.Add("placeholder", labelText);

            if (htmlAttributes.Where(m => m.Key == "style").Count() == 0)
                htmlAttributes.Add("style", "width: 250px");

            return html.TextBoxFor(expression, htmlAttributes);
        }
        public static MvcHtmlString KendoTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, HtmlHelperExtensions.KendoTextBoxType? textBoxType = HtmlHelperExtensions.KendoTextBoxType.Standard, object htmlAttributes = null)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            return KendoTextBoxFor(htmlHelper, name, metadata.Model as string, textBoxType, htmlAttributes);
        }

        public static MvcHtmlString KendoTextBoxFor(this HtmlHelper htmlHelper, string name, string value, HtmlHelperExtensions.KendoTextBoxType? textBoxType = HtmlHelperExtensions.KendoTextBoxType.Standard, object htmlAttributes = null)
        {
            var builder = new TagBuilder("input");
            builder.Attributes["type"] = "text";
            builder.Attributes["name"] = name;
            builder.Attributes["id"] = name;
            builder.Attributes["value"] = value;
            builder.Attributes["class"] = "form-control";

            if (textBoxType.HasValue)
            {
                if (textBoxType.Value == HtmlHelperExtensions.KendoTextBoxType.Password)
                    builder.Attributes["type"] = "password";
            }

            Dictionary<String, Object> attributes = new Dictionary<String, Object>();
            if (htmlAttributes != null)
            {
                System.Reflection.PropertyInfo[] properties = htmlAttributes.GetType().GetProperties();
                foreach (System.Reflection.PropertyInfo propertyInfo in properties)
                {
                    if (!propertyInfo.Name.Equals("class"))
                        builder.Attributes[propertyInfo.Name] = propertyInfo.GetValue(htmlAttributes, null).ToString();
                }
            }
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }

}