﻿using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace WpfTestApp.ViewModels
{
	public class NavigationTreeViewModel : BindableBase, IDisposable
	{
		/// <summary>TreeViewItem を取得します。</summary>
		public ReadOnlyReactiveCollection<TreeViewItemViewModel> TreeNodes { get; }

		/// <summary>SelectedItemChangedイベントハンドラ。</summary>
		public ReactiveCommand<System.Windows.RoutedPropertyChangedEventArgs<object>> SelectedItemChanged { get; }

		private WpfTestAppData appData = null;
		private TreeViewItemViewModel rootNode = null;
		private Prism.Regions.IRegionManager regionManager = null;
		/// <summary>ReactivePropertyのDispose用リスト</summary>
		private System.Reactive.Disposables.CompositeDisposable disposables
			= new System.Reactive.Disposables.CompositeDisposable();

		/// <summary>コンストラクタ。</summary>
		/// <param name="data">アプリのデータオブジェクト（Unity からインジェクション）</param>
		/// <param name="rm">IRegionManager（Unity からインジェクション）</param>
		public NavigationTreeViewModel(WpfTestAppData data, Prism.Regions.IRegionManager rm)
		{
			this.appData = data;
			this.regionManager = rm;

			this.rootNode = TreeViewItemCreator.Create(this.appData);
			var col = new System.Collections.ObjectModel.ObservableCollection<TreeViewItemViewModel>();

			col.Add(this.rootNode);
			this.TreeNodes = col.ToReadOnlyReactiveCollection()
				.AddTo(this.disposables);

			this.SelectedItemChanged = new ReactiveCommand<System.Windows.RoutedPropertyChangedEventArgs<object>>()
				.WithSubscribe(e => this.nodeChanged(e))
				.AddTo(this.disposables);

			//	.AddTo(this.disposables);
			//this.SelectedItemChanged.Subscribe(e => this.nodeChanged(e));
		}

		/// <summary>SelectedItemChangedイベントハンドラ。</summary>
		/// <param name="e">イベントデータを格納しているRoutedPropertyChangedEventArgs<object>。</param>
		private void nodeChanged(System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			var viewName = string.Empty;
			var current = e.NewValue as TreeViewItemViewModel;

			switch (current.SourceData)
			{
				case PersonalInformation p:
					viewName = "PersonalEditor";
					break;
				case PhysicalInformation p:
					viewName = "PhysicalEditor";
					break;
				case TestPointInformation t:
					viewName = "TestPointEditor";
					break;
				case string s:
					viewName = "CategoryPanel";
					break;
			}

			this.regionManager.RequestNavigate("EditorArea", viewName);
		}

		void IDisposable.Dispose() { this.disposables.Dispose(); }
	}
}
