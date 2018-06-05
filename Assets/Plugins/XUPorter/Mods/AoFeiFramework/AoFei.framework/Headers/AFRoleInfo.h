//
//  AFRoleInfo.h
//  AOFEISDK
//
//  Created by rowling on 2016/10/9.
//  Copyright © 2016年 AoFei JoyGames. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface AFRoleInfo : NSObject

#pragma mark 角色信息 
@property (nonatomic, copy) NSString *roleId;
@property (nonatomic, copy) NSString *roleName;
@property (nonatomic, copy) NSNumber *roleLevel;
@property (nonatomic, copy) NSString *serverId;
@property (nonatomic, copy) NSString *serverName;


@end
