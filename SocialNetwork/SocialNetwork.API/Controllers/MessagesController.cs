﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Contracts;
using SocialNetwork.BLL.DTO.Messages.Request;
using SocialNetwork.BLL.DTO.Messages.Response;
using SocialNetwork.BLL.Exceptions;

namespace SocialNetwork.API.Controllers;

[ApiController, Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }
    
    /// <summary>Reply Message</summary>
    /// <remarks>Reply chat message (for chat members).</remarks>
    /// <param name="messageId">The ID of the message to reply.</param>
    /// <param name="messageRequestDto">The message request data transfer object.</param>
    /// <response code="200">Returns a <see cref="MessageResponseDto"/> with the details of the newly created replied message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="403">Returns a string message if the user isn't chat member.</response>
    /// <response code="404">Returns a string message if the message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpPost, Route("{messageId}")]
    public virtual async Task<ActionResult<MessageResponseDto>> PostMessagesMessageId(
        [FromRoute, Required] uint messageId,
        [FromBody, Required] MessageRequestDto messageRequestDto)
    {
        var userId = (uint)HttpContext.Items["UserId"]!;
        try
        {
            var repliedMessageDto = await _messageService.ReplyMessage(userId, messageId, messageRequestDto);
            return Ok(repliedMessageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
        catch (AccessDeniedException accessDeniedException)
        {
            return Forbid(accessDeniedException.Message);
        }
    }
    
    /// <summary>Get Message</summary>
    /// <remarks>Get information about chat message.</remarks>
    /// <param name="messageId">The ID of the chat message.</param>
    /// <response code="200">Returns a <see cref="MessageResponseDto"/> with the details of the chat message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpGet, Route("{messageId}")]
    public virtual async Task<ActionResult<MessageResponseDto>> GetMessagesMessageId([FromRoute, Required] uint messageId)
    {
        try
        {
            var messageDto = await _messageService.GetMessage(messageId);
            return Ok(messageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
    }

    /// <summary>Change Message</summary>
    /// <remarks>Change chat message (for message senders).</remarks>
    /// <param name="messageId">The ID of the message to reply.</param>
    /// <param name="messagePatchRequestDto">The message patch request data transfer object.</param>
    /// <response code="200">Returns a <see cref="MessageResponseDto"/> with the details of the changed message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpPatch, Route("{messageId}")]
    public virtual async Task<ActionResult<MessageResponseDto>> PatchMessagesMessageId(
        [FromRoute, Required] uint messageId,
        [FromBody, Required] MessagePatchRequestDto messagePatchRequestDto)
    {
        var userId = (uint)HttpContext.Items["UserId"]!;
        try
        {
            var changedMessageDto = await _messageService.ChangeMessage(userId, messageId, messagePatchRequestDto);
            return Ok(changedMessageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
    }
    
    /// <summary>Delete Chat Message</summary>
    /// <remarks>Delete chat message (for message senders, chat admins, admins).</remarks>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <response code="200">Returns a <see cref="MessageResponseDto"/> with the details of the deleted message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="403">Returns a string message if the user can't delete the chat message.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpDelete, Route("{messageId}")]
    public virtual async Task<ActionResult<MessageResponseDto>> DeleteMessagesMessageId([FromRoute, Required] uint messageId)
    {
        var userId = (uint)HttpContext.Items["UserId"]!;
        try
        {
            var deletedMessageDto = await _messageService.DeleteMessage(userId, messageId);
            return Ok(deletedMessageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
        catch (AccessDeniedException accessDeniedException)
        {
            return Forbid(accessDeniedException.Message);
        }
    }
    
    /// <summary>Get All Message Likes</summary>
    /// <remarks>Get all likes of the message using pagination (for chat members).</remarks>
    /// <param name="messageId">The ID of the message to retrieve message likes for.</param>
    /// <param name="limit">The maximum number of message likes to retrieve.</param>
    /// <param name="nextCursor">The cursor value for pagination.</param>
    /// <response code="200">Returns a list of <see cref="MessageLikeResponseDto"/> with the details of the each message like.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpGet, Route("{messageId}/likes")]
    public virtual async Task<ActionResult<List<MessageLikeResponseDto>>> GetMessagesMessageId(
        [FromRoute, Required] uint messageId,
        [FromQuery, Required] int limit,
        [FromQuery, Required] int nextCursor)
    {
        try
        {
            var messageLikesDto = await _messageService.GetAllMessageLikesPaginated(messageId, limit, nextCursor);
            return Ok(messageLikesDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
    }

    /// <summary>Like Message</summary>
    /// <remarks>Like chat message (for chat members).</remarks>
    /// <param name="messageId">The ID of the message to like.</param>
    /// <response code="200">Returns a <see cref="MessageLikeResponseDto"/> with the details of the liked message.</response>
    /// <response code="400">Returns a string message if the user already like the message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="403">Returns a string message if the user isn't chat member of the chat contains the message.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpPost, Route("{messageId}/likes")]
    public virtual async Task<ActionResult<MessageLikeResponseDto>> PostMessagesMessageIdLikes(
        [FromRoute, Required] uint messageId)
    {
        var userId = (uint)HttpContext.Items["UserId"]!;
        try
        {
            var likedMessageDto = await _messageService.LikeMessage(userId, messageId);
            return Ok(likedMessageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
        catch (AccessDeniedException accessDeniedException)
        {
            return Forbid(accessDeniedException.Message);
        }
        catch (DuplicateEntryException duplicateEntryException)
        {
            return BadRequest(duplicateEntryException.Message);
        }
    }
    
    /// <summary>Unlike Message</summary>
    /// <remarks>Unlike chat message (for like owner).</remarks>
    /// <param name="messageId">The ID of the message to unlike.</param>
    /// <response code="200">Returns a <see cref="MessageLikeResponseDto"/> with the details of the unliked message.</response>
    /// <response code="400">Returns a string message if the user doesn't like the message.</response>
    /// <response code="401">Returns a string message if the user is unauthorized.</response>
    /// <response code="403">Returns a string message if the user isn't chat member of the chat contains the message.</response>
    /// <response code="404">Returns a string message if the chat message not founded.</response>
    [Authorize(Roles = "User")]
    [HttpDelete, Route("{messageId}/likes")]
    public virtual async Task<ActionResult<MessageLikeResponseDto>> DeleteMessagesMessageIdLikes([FromRoute, Required] uint messageId)
    {
        var userId = (uint)HttpContext.Items["UserId"]!;
        try
        {
            var unlikedMessageDto = await _messageService.UnlikeMessage(userId, messageId);
            return Ok(unlikedMessageDto);
        }
        catch (NotFoundException notFoundException)
        {
            return NotFound(notFoundException.Message);
        }
        catch (AccessDeniedException accessDeniedException)
        {
            return Forbid(accessDeniedException.Message);
        }
        catch (DuplicateEntryException duplicateEntryException)
        {
            return BadRequest(duplicateEntryException.Message);
        }
    }
}