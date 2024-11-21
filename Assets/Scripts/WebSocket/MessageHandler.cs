using System;
using System.Collections.Generic;
using System.Linq;

public static class MessageHandler
{
    public static void HandleChat(dynamic data, string username)
    {
        var message = new
        {
            type = "chat",
            avatar = data.profilePictureUrl,
            userId = data.userId,
            uniqueId = data.uniqueId,
            nickname = data.nickname,
            followerCount = data.followInfo.followerCount,
            followingCount = data.followInfo.followingCount,
            isModerator = data.isModerator,
            gifterLevel = data.gifterLevel,
            teamMemberLevel = data.teamMemberLevel,
            comment = data.comment,
            followRole = data.followRole,
            isSubscriber = data.isSubscriber
        };
        SendMessage(username, message);
    }

    public static void HandleGift(dynamic data, string username)
    {
        var giftMessage = new
        {
            type = "gift",
            avatar = data?.profilePictureUrl,
            userId = data.userId,
            uniqueId = data?.uniqueId,
            nickname = data?.nickname,
            followerCount = data?.followInfo?.followerCount,
            followingCount = data?.followInfo?.followingCount,
            isModerator = data?.isModerator,
            gifterLevel = data?.gifterLevel,
            teamMemberLevel = data?.teamMemberLevel,
            giftId = data?.giftId,
            giftName = data?.giftName,
            repeatEnd = data?.repeatEnd,
            giftType = data?.giftType,
            giftPictureUrl = data?.giftPictureUrl,
            diamondCount = data?.diamondCount,
            repeatCount = data?.repeatCount,
            createTime = data?.createTime,
            followRole = data?.followRole,
            isSubscriber = data?.isSubscriber
        };
        SendMessage(username, giftMessage);
    }

    public static void HandleMember(dynamic data, string username)
    {
        var member = new
        {
            type = "join",
            avatar = data.profilePictureUrl,
            userId = data.userId,
            uniqueId = data.uniqueId,
            nickname = data.nickname,
            followerCount = data.followInfo.followerCount,
            followingCount = data.followInfo.followingCount,
            isModerator = data.isModerator,
            gifterLevel = data.gifterLevel,
            teamMemberLevel = data.teamMemberLevel,
            followRole = data.followRole,
            isSubscriber = data.isSubscriber
        };
        SendMessage(username, member);
    }

    public static void HandleLike(dynamic data, string username)
    {
        var messageData = new
        {
            type = "like",
            userId = data.userId,
            avatar = data.profilePictureUrl,
            uniqueId = data.uniqueId,
            nickname = data.nickname,
            followerCount = data.followInfo.followerCount,
            followingCount = data.followInfo.followingCount,
            isModerator = data.isModerator,
            gifterLevel = data.gifterLevel,
            teamMemberLevel = data.teamMemberLevel,
            likeCount = data.likeCount,
            totalLikeCount = data.totalLikeCount,
            followRole = data.followRole,
            isSubscriber = data.isSubscriber
        };
        SendMessage(username, messageData);
    }

    public static void HandleViewerCount(dynamic data, string uid)
    {
        var viewerData = new
        {
            type = "viewercount",
            viewerCount = data.viewerCount,
            topViewers = ((IEnumerable<dynamic>)data.topViewers)
                         .Where(item => item.coinCount > 0)
        };
        SendMessage(uid, viewerData);
    }

    public static void HandleShare(dynamic data, string uid)
    {
        var shareData = new
        {
            type = "share",
            avatar = data.profilePictureUrl,
            userId = data.userId,
            uniqueId = data.uniqueId,
            nickname = data.nickname,
            followerCount = data.followInfo.followerCount,
            followingCount = data.followInfo.followingCount,
            isModerator = data.isModerator,
            userBadges = ((IEnumerable<dynamic>)data.userBadges).Select(badge => new
            {
                type = badge.type,
                name = badge.name ?? "",
                url = badge.url ?? ""
            }),
            createTime = data.createTime,
            msgId = data.msgId,
            displayType = data.displayType,
            label = data.label,
            followRole = data.followRole,
            isSubscriber = data.isSubscriber
        };
        SendMessage(uid, shareData);
    }

    public static void HandleFollow(dynamic data, string uid)
    {
        var followData = new
        {
            type = "follow",
            avatar = data.profilePictureUrl,
            userId = data.userId,
            uniqueId = data.uniqueId,
            nickname = data.nickname,
            followerCount = data.followInfo.followerCount,
            followingCount = data.followInfo.followingCount,
            isModerator = data.isModerator,
            userBadges = ((IEnumerable<dynamic>)data.userBadges).Select(badge => new
            {
                type = badge.type,
                name = badge.name ?? "",
                url = badge.url ?? ""
            }),
            createTime = data.createTime,
            msgId = data.msgId,
            displayType = data.displayType,
            label = data.label,
            followRole = data.followRole,
            isSubscriber = data.isSubscriber
        };
        SendMessage(uid, followData);
    }

    public static void HandleSubscribe(dynamic data, string uid)
    {
        var subscribeData = new
        {
            type = "subscribe",
            userId = data.userId,
            avatar = data.profilePictureUrl,
            uniqueId = data.uniqueId,
            nickname = data.nickname
        };
        SendMessage(uid, subscribeData);
    }

    private static void SendMessage(string recipient, object message)
    {
        // Placeholder for sending the message, e.g., over a WebSocket connection.
        Console.WriteLine($"Sending message to {recipient}: {message}");
    }
}
