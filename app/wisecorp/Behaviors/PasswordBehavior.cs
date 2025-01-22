using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using wisecorp.ViewModels;

namespace wisecorp.Behaviors
{
    //S'occupe du lien entre la password box et le view model
    public static class PasswordBehavior
    {

        public static readonly DependencyProperty IsAttachedProperty =
        DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(PasswordBehavior), new PropertyMetadata(false, OnIsAttachedChanged));

        /// <summary>
        /// Obtient la valeur de la propriété IsAttached pour l'objet spécifié
        /// </summary>
        /// <param name="obj">L'objet dont on veut obtenir la valeur</param>
        /// <returns>La valeur booléenne de IsAttached</returns>
        public static bool GetIsAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAttachedProperty);
        }

        /// <summary>
        /// Définit la valeur de la propriété IsAttached pour l'objet spécifié
        /// </summary>
        /// <param name="obj">L'objet dont on veut définir la valeur</param>
        /// <param name="value">La nouvelle valeur à attribuer</param>
        public static void SetIsAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAttachedProperty, value);
        }

        /// <summary>
        /// Gère le changement de la propriété IsAttached
        /// </summary>
        /// <param name="d">L'objet dont la propriété a changé</param>
        /// <param name="e">Les arguments de l'événement de changement</param>
        private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox == null)
                return;

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
            else
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            }
        }
        /// <summary>
        /// Gère l'événement de changement de mot de passe
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement</param>
        /// <param name="e">Les arguments de l'événement</param>
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
                return;

            if (passwordBox.DataContext is VMLogin viewModel)
            {
                viewModel.SetPassword(passwordBox.SecurePassword);
            }
        }
    }
}
