moveSpeed = 0.25

flying = false
player = nil
linkToDream = GetDreamByName("Pit & Temple")
audioPlayer = GetEntity("PterodactylAudio").DreamAudio

function start()
    if not IsDayNumber(3) then
        --this.GameObject.SetActive(false)
        --return
    end

    player = GetEntity("__player")
end

function update()
    if not flying then
        return
    end

    -- TODO: sometimes we don't fly

    local playerHead = player.WorldPosition + Unity.Vector3(0, 0.5, 0)
    this.MoveTowards(playerHead, moveSpeed)
    local distanceToPlayer = (playerHead - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.1 then
        flying = false
        DreamSystem.SetNextTransitionDream(linkToDream)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.PlayAnimation(0)
    SetCanControlPlayer(false)
    DreamSystem.LogGraphContributionFromEntity(-2, 5)
    audioPlayer.Play()
    this.LookAt(player.WorldPosition + Unity.Vector3(0, 0.5, 0))
    flying = true
end