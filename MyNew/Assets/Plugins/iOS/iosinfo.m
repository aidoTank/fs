//
//  iosinfo.m
//  Unity-iPhone
//
//  Created by yuelue on 17/6/12.
//
//

#import <Foundation/Foundation.h>
float GetiOSBatteryLevel(){
    [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
    return [[UIDevice currentDevice] batteryLevel];
}