//
//  Mp3Play.m
//  Mp3Play
//
//  Created by eyesblack on 16/1/2.
//  Copyright © 2016年 eyesblack. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AVFoundation/AVFAudio.h>

@interface Mp3Play : NSObject<AVAudioPlayerDelegate>
{
    AVAudioPlayer* player;
}
+ (instancetype) sharedInstance;
-(BOOL) isPlaying;
-(BOOL) Play:(NSString*)path;
-(BOOL) Stop;
-(BOOL) Pause;
-(BOOL) PlayByPause;
@end

@implementation Mp3Play

+ (instancetype) sharedInstance
{
    static Mp3Play  * instance = nil;
    static dispatch_once_t predict;
    dispatch_once(&predict, ^{
        instance = [Mp3Play alloc];
    });
    return instance;
}

-(BOOL) isPlaying
{
    if (!player)
        return false;
    
    return [player isPlaying];
}

-(BOOL) Play:(NSString *)path
{
    [self Stop];
    
    NSURL *url = [NSURL fileURLWithPath:path];
    player = [[AVAudioPlayer alloc] initWithContentsOfURL:url error:nil];
    
    AVAudioSession *session = [AVAudioSession sharedInstance];
    NSError *errorcode;
    if (![session overrideOutputAudioPort:AVAudioSessionPortOverrideSpeaker error:&errorcode])
    {
        NSLog(@"error doing outputaudioportoverride - %@", errorcode);
    }
    
    player.delegate = self;
    player.numberOfLoops = 0;
    player.volume = 1;
    [player prepareToPlay];
    [player play];
    
    return true;
}

//播放完成时调用的方法  (代理里的方法),需要设置代理才可以调用
-(void)audioPlayerDidFinishPlaying:(AVAudioPlayer *)player successfully:(BOOL)flag
{
    self->player = NULL;
}

-(BOOL) Stop
{
    if (player == NULL)
        return false;
    
    [player stop];
    player = NULL;
    return  true;
}

-(BOOL) Pause
{
    if (player == NULL)
        return false;
    [player pause];
    return true;
}

-(BOOL) PlayByPause
{
    if (player == NULL)
        return false;
    
    [player play];
    return true;
}
@end

extern "C"
{
    static NSString* __makeNSString(const char* cstring)
    {
        if (cstring == NULL) {
            return nil;
        }
        
        NSString* nsstring = [[NSString alloc] initWithCString:cstring encoding:NSUTF8StringEncoding];
        
        return nsstring;
    }
    
    bool Mp3Play_Pause()
    {
        return [[Mp3Play sharedInstance] Pause];
    }
    bool Mp3Play_PlayByPause()
    {
        return [[Mp3Play sharedInstance] PlayByPause];
    }
    bool Mp3Play_IsPlaying()
    {
        return [[Mp3Play sharedInstance] isPlaying];
    }
    bool Mp3Play_Play(char* filepath)
    {
        return [[Mp3Play sharedInstance] Play:__makeNSString(filepath)];
    }
    
    bool Mp3Play_Stop()
    {
        return [[Mp3Play sharedInstance] Stop];
    }
}

















