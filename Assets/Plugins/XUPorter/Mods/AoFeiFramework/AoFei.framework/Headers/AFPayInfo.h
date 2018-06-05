//
//  AFPayInfo.h
//  AOFEISDK
//
//  Created by rowling on 2016/10/9.
//  Copyright © 2016年 AoFei JoyGames. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface AFPayInfo : NSObject

#pragma mark 订单信息
@property (nonatomic, copy)     NSString *gameOrderNO;   //游戏订单号
@property (nonatomic, copy)     NSString *productId;     //商品id
@property (nonatomic, copy)     NSString *productName;   //商品名称
@property (nonatomic, copy)     NSString *productDesc;   //商品描述
@property (nonatomic, copy)     NSString *productExt;    //扩展字段
@property (nonatomic, copy)     NSString *sdkExt;        //时间戳,外部可不传
@property (nonatomic, copy)     NSString *currency;      //支付所使用的币种,外部可不传,收到内购信息后内部获取
@property (nonatomic, copy)     NSNumber *price;         //商品配方价格,外部可不传,单位分
@property (nonatomic, copy)     NSNumber *amount;        //可获得的虚拟货币数量，月卡类填0
@property (nonatomic, copy)     NSString *orderID;       //CP不需要关心这个

@end
