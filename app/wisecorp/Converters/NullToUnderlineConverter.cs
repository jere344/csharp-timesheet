
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace wisecorp
{
    public class NullToUnderlineConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une valeur en une décoration de texte (soulignement)
        /// </summary>
        /// <param name="value">La valeur à convertir</param>
        /// <param name="targetType">Le type cible de la conversion</param>
        /// <param name="parameter">Un paramètre optionnel pour la conversion</param>
        /// <param name="culture">Les informations culturelles à utiliser pour la conversion</param>
        /// <returns>TextDecorations.Underline si la valeur n'est pas nulle, sinon null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? TextDecorations.Underline : null;
        }
        /// <summary>
        /// Convertit une décoration de texte en sa valeur d'origine (non implémenté)
        /// </summary>
        /// <param name="value">La valeur à convertir</param>
        /// <param name="targetType">Le type cible de la conversion</param>
        /// <param name="parameter">Un paramètre optionnel pour la conversion</param>
        /// <param name="culture">Les informations culturelles à utiliser pour la conversion</param>
        /// <returns>Cette méthode n'est pas implémentée et lève une exception</returns>
        /// <exception cref="NotImplementedException">Cette méthode n'est pas implémentée</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}