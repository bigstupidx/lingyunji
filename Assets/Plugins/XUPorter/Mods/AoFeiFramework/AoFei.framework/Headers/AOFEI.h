//
//  AOFEI.h
//  AOFEISDK
//
//  Created by rowling on 2016/10/9.
//  Copyright © 2016年 AoFei JoyGames. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AFManager.h"
#import "AFInitConfig.h"
#import "AFRoleInfo.h"
#import "AFPayInfo.h"

@class UIApplication;


@interface AOFEI : NSObject

/*
 debugMode=YES的时候，该方法才有用
 */
+ (void)debugDeleteUnCompleteOrder;

/*
 字符串转NSDictionary
 */
+ (NSDictionary *)dictionaryFromString:(NSString *)string;

/*
 NSDictionary转JSON，针对Unity-3D接入提供
 */
+ (NSString *)jsonFromDictionary:(NSDictionary *)dic;


/**
 *  getUdid
 *
 */
+ (NSString *)getUdid;

/**
 *  初始化方法
 *
 *  param config
 */
+ (void)initWithConfig:(AFInitConfig *)config;

/**
 *  登录
 *
 */
+ (void)login;

/**
 *  登出
 *
 */
+ (void)logOut;

/**
 *  发起支付
 *
 *  param roleInfo
 */
+ (void)payWithRoleInfo:(AFRoleInfo *)roleInfo payInfo:(AFPayInfo *)payInfo;


/**
 *  其他支付结果处理
 *
 */
+ (void)wftPayResultHandler:(UIApplication *)application;

@end
