//
//  AFManager.h
//  AOFEISDK
//
//  Created by rowling on 2016/10/10.
//  Copyright © 2016年 AoFei JoyGames. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "AFInitConfig.h"
#import "AFRoleInfo.h"
#import "AFPayInfo.h"

static NSString * const kAFInitCompletionNotification     = @"kAFInitCompletionNotification";  /*初始化完成*/
static NSString * const kAFUnInitNotification             = @"kAFUnInitNotification";       /*未初始化通知*/
static NSString * const kAFLoginSuccessNotification       = @"kAFLoginSuccessNotification"; /*登录成功*/
static NSString * const kAFLoginErrorNotification         = @"kAFLoginErrorNotification";   /*登录错误*/
static NSString * const kAFUnLoginNotification            = @"kAFUnLoginNotification";      /*未登陆*/
static NSString * const kAFLogoutSuccessNotification      = @"kAFLogoutSuccessNotification"; /*登出成功*/
static NSString * const kAFOrderSuccessNotification       = @"kAFOrderSuccessNotification";    /*购买完成*/
static NSString * const kAFOrderErrorNotification         = @"kAFOrderErrorNotification";       /*购买失败*/

@interface AFManager : NSObject

@property (nonatomic, strong) UIView *rootView;
@property (nonatomic, strong) AFPayInfo *payInfo;
@property (nonatomic, strong) AFRoleInfo *roleInfo;
@property (nonatomic, strong) NSMutableDictionary *loginDic;
@property (nonatomic, assign) BOOL userInitiativeBuy;
@property (nonatomic, assign) BOOL isTouristPayOpen;
@property (nonatomic, assign) BOOL isOtherPayOpen;

//#ifdef ZZ
@property (nonatomic, assign) BOOL isAlipayOpen;
@property (nonatomic, assign) BOOL isIAPOpen;
@property (nonatomic, assign) BOOL isWechatPayOpen;
//#endif

+ (AFManager *)sharedInstance;

- (void)initWithConfig:(AFInitConfig *)config;
- (void)login;
- (void)logOut;
- (void)payWithRoleInfo:(AFRoleInfo *)roleInfo payInfo:(AFPayInfo *)payInfo;

//post notification
- (void)postInitComplete;
- (void)postUnInit;
- (void)postUnLogin;
- (void)postLoginSuccess:(NSDictionary *)resultDic;
- (void)postLoginErrorMessage:(NSString *)errorMsg;
- (void)postOrderError:(NSString *)errorMsg;
- (void)postOrderError:(NSString *)errorMsg result:(NSDictionary *)dic;
- (void)postOrderSuccess:(id)payInfo result:(NSDictionary *)dic;

- (NSDictionary *)payResult:(AFPayInfo *)payInfo state:(NSNumber *)state;

//控制悬浮窗
- (void)AFHideFloatWindow;
- (void)AFShowFloatWindow;
- (void)AFOpenIDAuthView;
- (void)AFUserCenterSwitchAccount;
- (void)AFUserCenterBindPhoneNo:(NSString *)username;
- (void)AFUserCenterChangePwd:(NSString *)username;
- (void)AFShowWelcome:(NSDictionary *)resultDic;
- (void)AFRootViewDidRemoveFromSuperViewHandler:(void (^)(void))handler;

- (void)debugDeleteUnCompleteOrder;

@end
