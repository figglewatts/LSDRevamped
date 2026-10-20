function start()
    this.PlayAnimation(0)
    
    if IsDayEven() or IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end
end