﻿/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public Transform weaponHold;
	public Gun startingGun;
	private Gun _equippedGun;

	void Start()
	{
		if(startingGun != null)
		{
			EquipGun(startingGun);
		}
	}

	public void EquipGun(Gun gunToEquip)
	{
		if(_equippedGun != null)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
			_equippedGun.transform.parent = weaponHold;	
		}
	}

	public void Shoot()
	{
		if(_equippedGun != null)
		{
			_equippedGun.Shoot();
		}
	}
}
