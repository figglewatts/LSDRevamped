require "dreams"

moveSpeed = 0.25

flying = false
player = nil
audioPlayer = GetEntity("PterodactylAudio").DreamAudio

function start()
    if not IsWeekDay(3) then
        this.GameObject.SetActive(false)
        return
    end

    player = GetEntity("__player")
end

function update()
    if not flying then
        return
    end

    local playerHead = player.WorldPosition + Unity.Vector3(0, 0.5, 0)
    this.MoveTowards(playerHead, moveSpeed)
    local distanceToPlayer = (playerHead - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 0.1 then
        flying = false
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    -- we have a 50% chance of not flying
    if Random.OneIn(2) then
        return
    end

    this.PlayAnimation(0)
    SetCanControlPlayer(false)
    DreamSystem.LogGraphContributionFromEntity(-2, 5)
    audioPlayer.Play()
    local lookAtPos = player.WorldPosition
    lookAtPos.y = this.GameObject.WorldPosition.y
    this.LookAt(lookAtPos)
    flying = true
end