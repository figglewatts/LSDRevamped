a = this.GameObject.GetChildByName("UnderwaterDudeOnWaterA").AnimatedObject
b = this.GameObject.GetChildByName("UnderwaterDudeOnWaterB").AnimatedObject

function start()
    a.Play(0)
    b.Play(0)
    
    if IsDayEven() or not IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end
end