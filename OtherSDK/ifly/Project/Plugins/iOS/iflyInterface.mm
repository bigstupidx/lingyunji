//
//  iflyInterface.c
//  MSCDemo
//
//  Created by eyesblack on 15/12/23.
//
//

#include <stdio.h>
#import "iflyMSC/IFlyMSC.h"
#import "UnityAppController.h"
#import "ISRDataHelper.h"
#import "IATConfig.h"

#define GameObjectName "SpeechRecognizer"

#if defined(__cplusplus)
extern "C"{
#endif
    extern void UnitySendMessage(const char *, const char *, const char *);
    
#if defined(__cplusplus)
}
#endif

@interface FlySpeechRecognizerDelegate : NSObject<IFlySpeechRecognizerDelegate>
+ (instancetype) sharedInstance;

@property IFlySpeechRecognizer *iFlySpeechRecognizer;

-(void) Init: (NSString*) appid;
-(void) InitFly: (NSString*) appid;
-(BOOL) Start;
-(BOOL) iflyInterface_isListening;
-(void) iflyInterface_Stop;
-(void) iflyInterface_Cancel;

/**
 * 音量变化回调
 * volume   录音的音量，音量范围0~30
 ****/
- (void) onVolumeChanged: (int)volume;

/**
 开始识别回调
 ****/
- (void) onBeginOfSpeech;

/**
 停止识别回调
 ****/
- (void) onEndOfSpeech;

/**
 识别结果回调（注：无论是否正确都会回调）
 error.errorCode =
 0     听写正确
 other 听写出错
 ****/
- (void) onError:(IFlySpeechError *) error;

/**
 识别结果回调
 result 识别结果，NSArray的第一个元素为NSDictionary，
 NSDictionary的key为识别结果，value为置信度
 isLast：表示最后一次
 ****/
- (void) onResults:(NSArray *) results isLast:(BOOL)isLast;

@end

static char* __makeCString(NSString* string)
{
    if (string == nil) {
        return NULL;
    }
    
    const char* cstring = [string cStringUsingEncoding:NSUTF8StringEncoding];
    
    if (NULL == cstring) {
        return NULL;
    }
    char* res = (char*)malloc(strlen(cstring)+1);
    strcpy(res, cstring);
    return res;
}

static NSString* __makeNSString(const char* cstring)
{
    if (cstring == NULL) {
        return nil;
    }
    
    NSString* nsstring = [[NSString alloc] initWithCString:cstring encoding:NSUTF8StringEncoding];
    
    return nsstring;
}

@implementation FlySpeechRecognizerDelegate

+ (instancetype) sharedInstance
{
    static FlySpeechRecognizerDelegate  * instance = nil;
    static dispatch_once_t predict;
    dispatch_once(&predict, ^{
        instance = [FlySpeechRecognizerDelegate alloc];
    });
    return instance;
}

-(void) InitFly : (NSString*) appid
{
    //设置sdk的log等级，log保存在下面设置的工作路径中
    [IFlySetting setLogFile:LVL_ALL];
    
    //打开输出在console的log开关
    [IFlySetting showLogcat:YES];
    
    //输出版本号
    NSLog(@"IFlyMSC version:%@",[IFlySetting getVersion]);
    
    //设置sdk的工作路径
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES);
    NSString *cachePath = [paths objectAtIndex:0];
    [IFlySetting setLogFilePath:cachePath];
    
    //创建语音配置,appid必须要传入，仅执行一次则可
    NSString *initString = [[NSString alloc] initWithFormat:@"appid=%@", appid];
    
    //所有服务启动前，需要确保执行createUtility
#pragma message "'createUtility' should be call before any business using."
    [IFlySpeechUtility createUtility:initString];
}

-(void) Init : (NSString*) appid
{
    //if (appid != NULL)
    //    return;

    [self InitFly : appid];
    
    //单例模式，无UI的实例
    if (_iFlySpeechRecognizer == nil)
    {
        _iFlySpeechRecognizer = [IFlySpeechRecognizer sharedInstance];
        
        [_iFlySpeechRecognizer setParameter:@"" forKey:[IFlySpeechConstant PARAMS]];
        
        //设置听写模式
        [_iFlySpeechRecognizer setParameter:@"iat" forKey:[IFlySpeechConstant IFLY_DOMAIN]];
    }
    
    _iFlySpeechRecognizer.delegate = self;
    
    if (_iFlySpeechRecognizer != nil) {
        IATConfig *instance = [IATConfig sharedInstance];
        //设置最长录音时间
        [_iFlySpeechRecognizer setParameter:instance.speechTimeout forKey:[IFlySpeechConstant SPEECH_TIMEOUT]];
        //设置后端点
        [_iFlySpeechRecognizer setParameter:instance.vadEos forKey:[IFlySpeechConstant VAD_EOS]];
        //设置前端点
        [_iFlySpeechRecognizer setParameter:instance.vadBos forKey:[IFlySpeechConstant VAD_BOS]];
        //设置采样率，推荐使用16K
        [_iFlySpeechRecognizer setParameter:instance.sampleRate forKey:[IFlySpeechConstant SAMPLE_RATE]];
        
        if ([instance.language isEqualToString:[IATConfig chinese]]) {
            //设置语言
            [_iFlySpeechRecognizer setParameter:instance.language forKey:[IFlySpeechConstant LANGUAGE]];
            //设置方言
            [_iFlySpeechRecognizer setParameter:instance.accent forKey:[IFlySpeechConstant ACCENT]];
        }else if ([instance.language isEqualToString:[IATConfig english]]) {
            [_iFlySpeechRecognizer setParameter:instance.language forKey:[IFlySpeechConstant LANGUAGE]];
        }
        //设置是否返回标点符号
        [_iFlySpeechRecognizer setParameter:instance.dot forKey:[IFlySpeechConstant ASR_PTT]];
    }
}

-(BOOL) Start
{
    //设置音频来源为麦克风
    [_iFlySpeechRecognizer setParameter:IFLY_AUDIO_SOURCE_MIC forKey:@"audio_source"];
    
    //设置听写结果格式为json
    [_iFlySpeechRecognizer setParameter:@"json" forKey:[IFlySpeechConstant RESULT_TYPE]];
    
    //保存录音文件，保存在sdk工作路径中，如未设置工作路径，则默认保存在library/cache下
    //[_iFlySpeechRecognizer setParameter:@"asr.pcm" forKey:[IFlySpeechConstant ASR_AUDIO_PATH]];
    
    //[_iFlySpeechRecognizer setDelegate:self];
    
    BOOL ret = [_iFlySpeechRecognizer startListening];
    return  ret;
}


-(BOOL) isListening
{
    return [_iFlySpeechRecognizer isListening];
}

-(void) Stop
{
    return [_iFlySpeechRecognizer stopListening];
}

-(void) Cancel
{
    return [_iFlySpeechRecognizer cancel];
}

/**
 * 音量变化回调
 * volume   录音的音量，音量范围0~30
 ****/
- (void) onVolumeChanged: (int)volume
{
    UnitySendMessage(GameObjectName, "onVolumeChanged", __makeCString([NSString stringWithFormat:@"%d", volume]));
}

/**
 开始识别回调
 ****/
- (void) onBeginOfSpeech
{
    UnitySendMessage(GameObjectName, "onBeginOfSpeech", "");
}

/**
 停止识别回调
 ****/
- (void) onEndOfSpeech
{
    UnitySendMessage(GameObjectName, "onEndOfSpeech", "");
}

/**
 识别结果回调（注：无论是否正确都会回调）
 error.errorCode =
 0     听写正确
 other 听写出错
 ****/
- (void) onError:(IFlySpeechError *) error;
{
    NSLog(@"%s error=%d",__func__,[error errorCode]);
    NSString* text;
    if (error.errorCode ==0 ) {
        text = @"识别成功";
    }
    else
    {
        text = [NSString stringWithFormat:@"发生错误：%d %@",error.errorCode,error.errorDesc];
        NSLog(@"%s %@",__func__,text);
    }
    
    UnitySendMessage(GameObjectName, "onError", __makeCString([NSString stringWithFormat:@"%d", error.errorCode]));
}

/**
 无界面，听写结果回调
 results：听写结果
 isLast：表示最后一次
 ****/
- (void) onResults:(NSArray *) results isLast:(BOOL)isLast
{
    NSMutableString *resultString = [[NSMutableString alloc] init];
    NSDictionary *dic = results[0];
    for (NSString *key in dic) {
        [resultString appendFormat:@"%@",key];
    }
    NSString * resultFromJson =  [ISRDataHelper stringFromJson:resultString];
    
    if (isLast){
        NSLog(@"听写结果(json)：%@测试", resultFromJson);
    }

    NSLog(@"Result:%@", resultFromJson);
    UnitySendMessage(GameObjectName, isLast ? "onResultEnd" : "onResult", __makeCString(resultFromJson));
}

@end

extern "C"
{
    void iflyInterface_Init(char* appid)
    {
        NSString* id = __makeNSString(appid);
        [[FlySpeechRecognizerDelegate sharedInstance] Init: id];
    }
    
    void iflyInterface_SetParameter(char* var1, char* var2)
    {
        //return;
        NSString *v1 = [[NSString alloc] initWithFormat:@"%s", var1];
        NSString *v2 = [[NSString alloc] initWithFormat:@"%s", var2];

        [[IFlySpeechUtility getUtility] setParameter:v2 forKey:v1];
    }
    
    BOOL iflyInterface_CheckServiceInstalled()
    {
        return false;
        //return [[IFlySpeechUtility getUtility] checkServiceInstalled];
    }
    
    char* iflyInterface_GetServiceUrl()
    {
        return __makeCString([IFlySpeechUtility componentUrl]);
    }
    
    int iflyInterface_Start()
    {
        return  [[FlySpeechRecognizerDelegate sharedInstance] Start];
    }
    
    BOOL iflyInterface_isListening()
    {
        return  [[FlySpeechRecognizerDelegate sharedInstance] isListening];
    }
    
    void iflyInterface_Stop()
    {
        [[FlySpeechRecognizerDelegate sharedInstance] Stop];
    }
    
    void iflyInterface_Cancel()
    {
        [[FlySpeechRecognizerDelegate sharedInstance] Cancel];
    }
}





















