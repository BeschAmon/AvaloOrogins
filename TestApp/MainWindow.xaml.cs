/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AvalonDock.Layout;
using System.Diagnostics;
using System.IO;
using AvalonDock.Layout.Serialization;
using AvalonDock;
using System.Diagnostics.CodeAnalysis;

namespace TestApp
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
		}

		private void OnShowPanel(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			string panelName = menuItem.Header.ToString();
			string contentId = panelName.Replace(" ", "") + (panelName.EndsWith("ы") ? "Pane" : "Document");

			var panel = dockManager.Layout.Descendents()
				.OfType<LayoutContent>()
				.FirstOrDefault(c => c.ContentId == contentId);

			if (panel == null)
			{
				MessageBox.Show($"Панель {panelName} не найдена");
				return;
			}

			if (panel is LayoutAnchorable anchorable)
			{
				if (anchorable.IsHidden)
					anchorable.Show();
				else if (anchorable.IsVisible)
					anchorable.IsActive = true;
			}
			else if (panel is LayoutDocument document)
			{
				document.IsActive = true;
			}
		}

		private void OnLoadLayout(object sender, RoutedEventArgs e)
		{
			try
			{
				var serializer = new XmlLayoutSerializer(dockManager);
				using (var stream = new StreamReader("layout.config"))
					serializer.Deserialize(stream);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка загрузки раскладки: {ex.Message}");
			}
		}

		private void OnSaveLayout(object sender, RoutedEventArgs e)
		{
			try
			{
				var serializer = new XmlLayoutSerializer(dockManager);
				using (var stream = new StreamWriter("layout.config"))
					serializer.Serialize(stream);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка сохранения раскладки: {ex.Message}");
			}
		}

		private void OnExit(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void DockManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
		{
			if (MessageBox.Show("Закрыть документ?", "Подтверждение",
				MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
			}
		}
	}
}