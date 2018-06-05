//
//  AFInitConfig.h
//  AOFEISDK
//
//  Created by rowling on 2016/10/9.
//  Copyright © 2016年 AoFei JoyGames. All rights reserved.
//

#import <Foundation/Foundation.h>

#pragma mark 初始化配置
@interface AFInitConfig : NSObject

@property (nonatomic, assign)   BOOL debugMode;
@property (nonatomic, copy)     NSString *appID;
@property (nonatomic, copy)     NSString *appKey;
@property (nonatomic, copy)     NSString *channelID;
@property (nonatomic, copy)     NSString *channelLabel;
@property (nonatomic, copy)     NSString *game;

- (void)checkParameterHandler:(void(^)(void))handler;

@end
