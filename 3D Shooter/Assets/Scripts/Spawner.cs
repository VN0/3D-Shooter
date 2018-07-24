﻿/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public event System.Action<int> OnNewWave;
	public Wave[] waves;
	public Enemy enemy;

	private LivingEntity playerEntity;
	private Transform playerT;

	private Wave _currentWave;
	private int _currentWaveNumber;
	private int _enemiesRemainingToSpawn;
	private int _enemiesRemainingAlive;
	private float _nextSpawnTime;

	MapGenerator map;

	private float timeBetweenCampingChecks = 2f;
	private float campThresholdDistance = 1.5f;
	private float nextCampCheckTime;
	private Vector3 campPositionOld;
	private bool isCamping;

	bool isDisabled;

	void Start()
	{
		isDisabled = false;

		playerEntity = FindObjectOfType<Player>();
		playerT = playerEntity.transform;

		nextCampCheckTime = timeBetweenCampingChecks + Time.time;
		campPositionOld = playerT.position;
		playerEntity.onDeath += OnPlayerDeath;

		map = FindObjectOfType<MapGenerator>();

		NextWave();
	}

	void Update()
	{
		if(!isDisabled)
		{		
			if(Time.time > nextCampCheckTime)
			{
				nextCampCheckTime = Time.time + timeBetweenCampingChecks;

				isCamping = (Vector3.Distance(playerT.position, campPositionOld)) < campThresholdDistance;

				campPositionOld = playerT.position;
			}


			if(_enemiesRemainingToSpawn > 0 && Time.time > _nextSpawnTime)
			{
				_enemiesRemainingToSpawn--;
				_nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

				StartCoroutine(SpawnEnemy());
			}
		}
	}

	IEnumerator SpawnEnemy()
	{
		float spawnDelay = 1f;
		float tileFlashSpeed = 4f;

		Transform spawnTile = map.GetRandomOpenTile();

		if(isCamping)
		{
			spawnTile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawnTile.GetComponent<Renderer>().material;
		Color initialColor = tileMat.color;
		Color flashColor = Color.red;
		float spawnTimer = 0f;

		while(spawnTimer < spawnDelay)
		{
			tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.onDeath += OnEnemyDeath;
	}

	void OnPlayerDeath()
	{
		isDisabled = true;
	}

	void OnEnemyDeath()
	{
		// print("Enemy died");
		_enemiesRemainingAlive--;

		if(_enemiesRemainingAlive == 0)
		{
			NextWave();
		}
	}

	void NextWave()
	{
		_currentWaveNumber++;
		
		if(_currentWaveNumber - 1 < waves.Length)
		{			_currentWave = waves[_currentWaveNumber - 1];

			_enemiesRemainingToSpawn = _currentWave.enemyCount;
			_enemiesRemainingAlive = _enemiesRemainingToSpawn;

			if(OnNewWave != null)
			{
				OnNewWave(_currentWaveNumber);
			}
		}
	}

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
