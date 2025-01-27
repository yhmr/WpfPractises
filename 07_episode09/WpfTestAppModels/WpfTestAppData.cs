﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WpfTestApp
{
	/// <summary>サンプルアプリのデータコンテナを表します。</summary>
	[System.Runtime.Serialization.DataContract]
	public class WpfTestAppData
	{
		#region "プロパティ"

		/// <summary>生徒の情報を取得・設定します。</summary>
		[System.Runtime.Serialization.DataMember]
		public PersonalInformation Student { get; set; } = new PersonalInformation();

		/// <summary>身体測定データを取得します。</summary>
		[System.Runtime.Serialization.DataMember]
		public System.Collections.ObjectModel.ObservableCollection<PhysicalInformation> Physicals { get; private set; }
			= new System.Collections.ObjectModel.ObservableCollection<PhysicalInformation>();

		/// <summary>試験結果データを取得します。</summary>
		[System.Runtime.Serialization.DataMember]
		public System.Collections.ObjectModel.ObservableCollection<TestPointInformation> TestPoints { get; private set; }
			= new System.Collections.ObjectModel.ObservableCollection<TestPointInformation>();

		#endregion

		/// <summary>新規データを作成します。</summary>
		/// <typeparam name="T">作成するデータの型を表します。</typeparam>
		/// <returns>作成した新規データを表すT。</returns>
		public T CreateNewData<T>() where T: class
		{
			if (typeof(T) == typeof(PhysicalInformation))
			{
				var id = (this.getMaxPhysicalId()) + 1;
				return new PhysicalInformation()
				{
					  Id = id
					//, MeasurementDate = DateTime.Now
				} as T;
			}

			if (typeof(T) == typeof(TestPointInformation))
			{
				var id = (this.getMaxTestPointId()) + 1;

				return new TestPointInformation()
				{
					Id = id,
					TestDate = "新しい試験日"
				} as T;
			}

			return null;
		}

		/// <summary>身体測定データIDの最大値を取得します。</summary>
		/// <returns>身体測定データIDの最大値を表すint。</returns>
		private int getMaxPhysicalId()
		{
			if (this.Physicals.Count == 0) { return 0; }

			return this.Physicals.Max(p => p.Id);
		}

		/// <summary>試験結果データIDの最大値を取得します。</summary>
		/// <returns>試験結果データIDの最大値を表すint。</returns>
		private int getMaxTestPointId()
		{
			if (this.TestPoints.Count == 0) { return 0; }

			return this.TestPoints.Max(p => p.Id);
		}

		/// <summary>指定した日付と同日の身体測定データ（自分自身は除く）が
		/// 存在するかを返します。</summary>
		/// <param name="value">存在をチェックする日付を表すDateTime?。</param>
		/// <param name="target">チェックで除外する身体測定データを表すPhysicalInformation。</param>
		/// <returns>指定した日付と同日の身体測定データが存在するかを表すbool。</returns>
		public bool HasPhysicalKey(DateTime? value, PhysicalInformation target)
		{
			if (value.HasValue)
				return this.Physicals
					.Where(p => p.MeasurementDate.HasValue)
					.FirstOrDefault((p) => p.MeasurementDate.Value.Date == value.Value.Date && p.Id != target.Id) != null;
			else
				return this.Physicals.FirstOrDefault(p => !p.MeasurementDate.HasValue) != null;
		}

		/// <summary>指定した試験日と同日の試験結果データ（自分自身は除く）が
		/// 存在するかを返します。</summary>
		/// <param name="value">存在をチェックする試験日を表す文字列。</param>
		/// <param name="target">チェックで除外する試験結果データを表すTestPointInformation。</param>
		/// <returns>指定した試験日と同じ値の試験結果データが存在するかを表すbool。</returns>
		public bool HasTestPointKey(string value, TestPointInformation target)
		{
			return this.TestPoints.FirstOrDefault(t => t.TestDate == value && t.Id != target.Id) != null;
		}
	}
}
