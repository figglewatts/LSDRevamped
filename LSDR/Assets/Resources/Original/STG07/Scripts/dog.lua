require "dreams"

player = GetEntity("__player")
audio = GetEntity("DogBarkAudio").DreamAudio

interacted = false
moveSpeed = 2

function start()
    this.SetChildVisible(false)
    
    if not IsWeekDay(4) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.4 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.SunFacesBridge)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not interacted then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    this.LogGraphContribution(0, -1)
    audio.Play()
    interacted = true
end