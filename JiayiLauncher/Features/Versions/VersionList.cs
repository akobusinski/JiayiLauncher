﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiayiLauncher.Features.Versions;

public static class VersionList
{
	private static Dictionary<string, MinecraftVersion> _versionDict = new();
	private static readonly List<string> _versions = new();

	private static async Task UpdateVersions()
	{
		using var client = new HttpClient();

		var response =
			await client.GetAsync("https://raw.githubusercontent.com/MinecraftBedrockArchiver/Metadata/master/w10_meta.json");
		
		if (!response.IsSuccessStatusCode) return;
		
		var content = await response.Content.ReadAsStringAsync();
		var json = JsonConvert.DeserializeObject<Dictionary<string, MinecraftVersion>>(content);
		
		if (json != null) _versionDict = json;
	}

	public static async Task<List<string>> GetVersionList()
	{
		if (_versions.Count > 0) return _versions;

		await UpdateVersions();

		foreach (var version in _versionDict.Keys)
		{
			_versions.Add(version);
		}
		
		// reverse the list so the newest version is first
		_versions.Reverse();

		return _versions;
	}
	
	public static async Task<MinecraftVersion> GetVersion(string version)
	{
		if (_versionDict.TryGetValue(version, out var mcVersion)) return mcVersion;
		
		await UpdateVersions();

		if (!_versions.Contains(version)) throw new ArgumentException("The version you specified does not exist.");
		return _versionDict[version];
	}
}