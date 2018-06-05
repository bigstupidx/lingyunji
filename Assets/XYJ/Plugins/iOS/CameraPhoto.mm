//
//  CameraPhoto.m
//  Unity-iPhone
//
//  Created by eyesblack on 2016/11/17.
//
//

#import <Foundation/Foundation.h>
#import <MobileCoreServices/MobileCoreServices.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

@interface UIImage (fixOrientation)
- (UIImage *)fixOrientation;
@end

@implementation UIImage (fixOrientation)

- (UIImage *)fixOrientation
{
    // No-op if the orientation is already correct
    if (self.imageOrientation == UIImageOrientationUp)
        return self;
    
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
    
    switch (self.imageOrientation)
    {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, self.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
            
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
            
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, self.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    
    switch (self.imageOrientation)
    {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
            
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    
    // Now we draw the underlying CGImage into a new context, applying the transform
    // calculated above.
    CGContextRef ctx = CGBitmapContextCreate(NULL, self.size.width, self.size.height,
                                             CGImageGetBitsPerComponent(self.CGImage), 0,
                                             CGImageGetColorSpace(self.CGImage),
                                             CGImageGetBitmapInfo(self.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (self.imageOrientation)
    {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            // Grr...
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.height,self.size.width), self.CGImage);
            break;
            
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.width,self.size.height), self.CGImage);
            break;
    }
    
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img;
}

@end

static char UnityObj[128];
static NSString* FilePath;
static int image_width;
static int image_height;

@interface CameraPhoto : NSObject<UIImagePickerControllerDelegate,UINavigationControllerDelegate>

+ (id)   sharedInstance;

-(void) PickImage:(UIImagePickerControllerSourceType) source;
- (UIImage*) EncodeImage:(UIImage *)image;

-(void) GetImageFromCamera;
-(void) GetImage: (UIImagePickerControllerSourceType )source;

@end


@implementation CameraPhoto

static CameraPhoto * cam_sharedInstance = NULL;
static UIImagePickerController *_imagePicker = NULL;


+ (id)sharedInstance
{
    if (cam_sharedInstance == nil)
    {
        cam_sharedInstance = [[self alloc] init];
    }
    
    return cam_sharedInstance;
}

-(void) PickImage:(UIImagePickerControllerSourceType)source
{
    if(source == UIImagePickerControllerSourceTypeCamera)
    {
        [self GetImageFromCamera];
    }
    else
    {
        [self GetImage:source];
    }
}

-(void) StartCameraImagePic
{
    [self GetImage:UIImagePickerControllerSourceTypeCamera];
}

-(void) GetImageFromCamera
{
    BOOL cameraAvailableFlag = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
    if (cameraAvailableFlag)
    {
        [self performSelector:@selector(StartCameraImagePic) withObject:nil afterDelay:0.9];
    }
}

-(void) GetImage: (UIImagePickerControllerSourceType )source
{
    UIViewController *vc =  UnityGetGLViewController();
    
    if(_imagePicker == NULL)
    {
        _imagePicker = [[UIImagePickerController alloc] init];
        _imagePicker.delegate = self;
    }
    
    _imagePicker.sourceType = source;
    _imagePicker.allowsEditing = YES;
    
    [vc presentViewController:_imagePicker animated:YES completion:nil];
}

-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    // added video support
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType]; // get media type
    // if mediatype is video
    if (CFStringCompare ((__bridge CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo)
    {
        NSURL *videoUrl=(NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
        NSString *moviePath = [videoUrl path];
        UnitySendMessage(UnityObj, "OnVideoPickedEvent", [moviePath UTF8String]);
    }
    else
    {
        // it must be an image
        //UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
        UIImage *photo = [info objectForKey:UIImagePickerControllerEditedImage];
        if (photo == nil)
        {
            UnitySendMessage(UnityObj, "OnEnd", "photo == nil");
            return;
        }
        else
        {
            UIImage* encodedImage = [self EncodeImage:photo];
            photo = nil;
            NSData* pngdata = UIImagePNGRepresentation(encodedImage);
            encodedImage = nil;
            [pngdata writeToFile:FilePath atomically:YES];
            
            UnitySendMessage(UnityObj, "OnEnd", "");
        }
    }
    
}

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize
{
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    [vc dismissModalViewControllerAnimated:YES];
    
    UnitySendMessage(UnityObj, "OnEnd", "Cancel");
}


- (UIImage*) EncodeImage:(UIImage *)image
{
    image = [image fixOrientation];
    CGSize size = [image size];
    NSLog(@"size:(%f,%f)", size.width, size.height);
    if (size.width != image_width || size.height != image_height)
    {
        size.width = image_width;
        size.height = image_height;

        image = [CameraPhoto imageWithImage:image scaledToSize:size];
    }

    return image;
}

@end

extern "C"
{
    void CameraPhoto_ShowCamera(char* unityobj, char* filepath, int width, int height)
    {
        strcpy(UnityObj, unityobj);
        image_width = width;
        image_height = height;

        FilePath = [[NSString alloc] initWithUTF8String:filepath];
        
        [[CameraPhoto sharedInstance]PickImage:UIImagePickerControllerSourceType::UIImagePickerControllerSourceTypeCamera];
    }
    
    void CameraPhoto_ShowPhoto(char* unityobj, char* filepath, int width, int height)
    {
        strcpy(UnityObj, unityobj);
        FilePath = [[NSString alloc] initWithUTF8String:filepath];
        image_width = width;
        image_height = height;
        
        [[CameraPhoto sharedInstance]PickImage:UIImagePickerControllerSourceType::UIImagePickerControllerSourceTypeSavedPhotosAlbum];
    }
    
    void CameraPhoto_ShowLibrary(char* unityobj, char* filepath, int width, int height)
    {
        strcpy(UnityObj, unityobj);
        FilePath = [[NSString alloc] initWithUTF8String:filepath];
        image_width = width;
        image_height = height;
        
        [[CameraPhoto sharedInstance]PickImage:UIImagePickerControllerSourceType::UIImagePickerControllerSourceTypePhotoLibrary];
    }
}






















