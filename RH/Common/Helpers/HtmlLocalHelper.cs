using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;


namespace Common.Helpers
{
    public static class HtmlLocalHelper
    { 

        public static MvcHtmlString ComboBox(List<SelectListItem> lista, string identificador, IDictionary<string, object> htmlAttributes = null, bool inputSM = true)
        {

            if (lista == null) return null;
            var combo = new TagBuilder("select");
            combo.Attributes.Add("name", identificador);
            combo.Attributes.Add("id", identificador);
            if(inputSM)
                combo.Attributes.Add("class", "form-control input-sm");
            else
                combo.Attributes.Add("class", "form-control");

            if (htmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> pair in htmlAttributes)
                {
                    combo.Attributes.Add(pair.Key,pair.Value.ToString());
                }
            }

            var cadHtml = new StringBuilder();

            foreach (var item in lista)
            {
                cadHtml.Append("<option value = \"" + item.Value + "\" " + (item.Selected ? "Selected" : " ") + " >" + item.Text + "</ option >");
            }

            combo.InnerHtml = cadHtml.ToString();

            var strHtml = combo.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(strHtml);
        }

        public static MvcHtmlString ComboBoxForEnum(List<SelectListItem> lista, string identificador, string valorSeleccionado, IDictionary<string, object> htmlAttributes = null)
        {

            var combo = new TagBuilder("select");
            combo.Attributes.Add("name", identificador);
            combo.Attributes.Add("id", identificador);
            combo.Attributes.Add("class", "form-control input-sm");

            if (htmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> pair in htmlAttributes)
                {
                    combo.Attributes.Add(pair.Key, pair.Value.ToString());
                }
            }

            var cadHtml = new StringBuilder();

            foreach (var item in lista)
            {
                cadHtml.Append("<option value = \"" + item.Value + "\" " + (item.Value == valorSeleccionado ? "Selected" : " ") + " >" + item.Text + "</ option >");
            }

            combo.InnerHtml = cadHtml.ToString();

            var strHtml = combo.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(strHtml);
        }

        /// <summary>
        /// Recibe un enumerador y regresa un select añadiendo una clase extra
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="identificador"></param>
        /// <param name="valorSeleccionado"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static MvcHtmlString ComboBoxForEnum(Type enumType, string identificador, string valorSeleccionado, string className)
        {

            var combo = new TagBuilder("select");
            combo.Attributes.Add("name", identificador);
            combo.Attributes.Add("id", identificador);
            combo.Attributes.Add("class", "form-control " + className);

            var valores = Enum.GetValues(enumType);
            var nombres = Enum.GetNames(enumType);
            bool selected = false;

            Type tipo = Enum.GetUnderlyingType(enumType);

            var cadHtml = new StringBuilder();

            for (int i = 0; i < valores.Length; i++)
            {

                var valor = Convert.ChangeType(valores.GetValue(i), tipo).ToString();

                if (valor == valorSeleccionado)
                    selected = true;

                cadHtml.Append("<option value = \"" + valor + "\" " + (selected ? "Selected" : "") + " >" + nombres[i].Replace("_", " ") + "</option>");

                selected = false;
            }


            combo.InnerHtml = cadHtml.ToString();

            var strHtml = combo.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(strHtml);
        }


        public static MvcHtmlString ComboEnum(Type enumType, string identificador, string valorSeleccionado, bool soloTexto = false, IDictionary<string, object> htmlAttributes = null, bool inputSM = true)
        {
            
            var valores = Enum.GetValues(enumType);
            var nombres = Enum.GetNames(enumType);
            bool selected = false;

            Type tipo = Enum.GetUnderlyingType(enumType);

            List<SelectListItem> lista = new List<SelectListItem>();

            for (int i = 0; i < valores.Length; i++)
            {

                var valor = soloTexto ? nombres[i] : Convert.ChangeType(valores.GetValue(i), tipo).ToString();

                if (valor == valorSeleccionado)
                    selected = true;


                lista.Add(new SelectListItem() {Value = valor, Text = nombres[i], Selected = selected});
                selected = false;
            }

            return ComboBox(lista, identificador, htmlAttributes, inputSM);
        }
    }
}
