interacted = false
swaying = false
swayTime = 0
swaySpeed = 0.3

function start()
    this.PlayAnimation(1)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if not activeOnDay(DreamSystem.DayNumber) then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if not swaying then return end

    swayTime = swayTime + Unity.DeltaTime()
    local direction = (swayTime % 1 < 0.5 and this.Right) or this.Right.negated()
    this.MoveInDirection(direction, swaySpeed)
end

function interact()
    this.LogGraphContribution(0, 5)

    if interacted then return end
    interacted = true

    -- some stretch into a thin needle, the rest sway about
    if Random.OneIn(2) then
        this.StopAnimation()
        this.GameObject.Scale = Unity.Vector3(0.125, 2, 0.125)
    else
        swaying = true
    end
end

function activeOnDay(day)
    local zeroDay = ((day / 2) % 3) - 1
    return zeroDay == 0
end