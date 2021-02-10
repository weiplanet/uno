﻿#if __ANDROID__
using System;
using System.Linq;
using System.Net;
using System.Security;
using Android;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Telecom;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.UI;
using Windows.Extensions;
using AndroidConnectivityManager = Android.Net.ConnectivityManager;

namespace Windows.Networking.Connectivity
{
	/// <summary>
	/// Current Android implementation covers the active network connection (Internet connection profile).
	/// </summary>
	public partial class ConnectionProfile
	{
		private AndroidConnectivityManager _connectivityManager;

		internal static ConnectionProfile GetInternetConnectionProfile() => new ConnectionProfile();

		private ConnectionProfile()
		{
			NetworkInformation.VerifyNetworkStateAccess();
			_connectivityManager = (AndroidConnectivityManager)ContextHelper.Current.GetSystemService(Context.ConnectivityService);
#pragma warning disable CS0618 // Type or member is obsolete
			NetworkInfo info = _connectivityManager.ActiveNetworkInfo;
			if (info?.IsConnected == true)
			{
				IsWwanConnectionProfile = IsConnectionWwan(info.Type);
				IsWlanConnectionProfile = IsConnectionWlan(info.Type);
			}
#pragma warning restore CS0618 // Type or member is obsolete
		}

		public ConnectionCost GetConnectionCost() =>
			new ConnectionCost(
				_connectivityManager.IsActiveNetworkMetered ?
					NetworkCostType.Fixed : // we currently don't make distinction between variable and fixed metered connection on Android
					NetworkCostType.Unrestricted);

		/// <summary>
		/// Code based on Xamarin.Essentials implementation with some modifications.
		/// </summary>		
		/// <returns>Connectivity level.</returns>
		private NetworkConnectivityLevel GetNetworkConnectivityLevelImpl()
		{
			try
			{
				var connectivityLevel = NetworkConnectivityLevel.None;
				var manager = _connectivityManager;

				if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
				{
					var networks = manager.GetAllNetworks();

					// some devices running 21 and 22 only use the older api.
					if (networks.Length == 0 && (int)Build.VERSION.SdkInt < 23)
					{
						return GetConnectivityFromManager();
					}

					foreach (var network in networks)
					{
						if (connectivityLevel == NetworkConnectivityLevel.InternetAccess)
						{
							// Internet access is the highest possible connectivity, short circuit
							return connectivityLevel;
						}

						try
						{
							var capabilities = manager.GetNetworkCapabilities(network);

							if (capabilities == null)
								continue;

#pragma warning disable CS0618 // Type or member is obsolete
							var info = manager.GetNetworkInfo(network);
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
							if (info == null || !info.IsAvailable)
#pragma warning restore CS0618 // Type or member is obsolete
								continue;

							// Check to see if it has the internet capability
							if (!capabilities.HasCapability(NetCapability.Internet))
							{
								// Doesn't have internet, but local is possible
								if (NetworkConnectivityLevel.LocalAccess > connectivityLevel)
								{
									connectivityLevel = NetworkConnectivityLevel.LocalAccess;
								}
								continue;
							}

							var networkConnectivity = GetConnectivityFromNetworkInfo(info);
							if (networkConnectivity > connectivityLevel)
							{
								connectivityLevel = networkConnectivity;
							}
						}
						catch
						{
							if (this.Log().IsEnabled(LogLevel.Debug))
							{
								this.Log().LogDebug($"Failed to access network information.");
							}
						}
					}
				}
				else
				{
					connectivityLevel = GetConnectivityFromManager();
				}

				return connectivityLevel;
			}
			catch (Exception)
			{
				if (this.Log().IsEnabled(LogLevel.Debug))
				{
					this.Log().LogDebug("Unable to get connected state, ensure network access permission is declared in manifest.");
				}
				return NetworkConnectivityLevel.None;
			}
		}

		private static bool IsConnectionWlan(ConnectivityType connectivityType)
		{
			return connectivityType == ConnectivityType.Wifi;
		}

		private static bool IsConnectionWwan(ConnectivityType connectivityType)
		{
			return
				connectivityType == ConnectivityType.Wimax ||
				connectivityType == ConnectivityType.Mobile ||
				connectivityType == ConnectivityType.MobileDun ||
				connectivityType == ConnectivityType.MobileHipri ||
				connectivityType == ConnectivityType.MobileMms ||
				connectivityType == ConnectivityType.MobileSupl;
		}

		private NetworkConnectivityLevel GetConnectivityFromManager()
		{
			NetworkConnectivityLevel bestConnectivityLevel = NetworkConnectivityLevel.None;
#pragma warning disable CS0618 // Type or member is obsolete
			foreach (var info in _connectivityManager.GetAllNetworkInfo())
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var networkConnectivityLevel = GetConnectivityFromNetworkInfo(info);
				if (networkConnectivityLevel > bestConnectivityLevel)
				{
					bestConnectivityLevel = networkConnectivityLevel;
				}
			}
			return bestConnectivityLevel;
		}

#pragma warning disable CS0618 // Type or member is obsolete
		private NetworkConnectivityLevel GetConnectivityFromNetworkInfo(NetworkInfo info)
		{
			if (info == null || !info.IsAvailable)
			{
				return NetworkConnectivityLevel.None;
			}
			if (info.IsConnected)
			{
				return NetworkConnectivityLevel.InternetAccess;
			}
			else if (info.IsConnectedOrConnecting)
			{
				return NetworkConnectivityLevel.ConstrainedInternetAccess;
			}
			return NetworkConnectivityLevel.None;
		}
#pragma warning restore CS0618 // Type or member is obsolete
	}

}

#endif
