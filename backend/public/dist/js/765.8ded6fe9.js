"use strict";(self["webpackChunkweb"]=self["webpackChunkweb"]||[]).push([[765],{2765:function(n,i,e){e.r(i),e.d(i,{default:function(){return g}});var d=e(3396),a=e(9242);const t=n=>((0,d.dD)("data-v-2e62b5e4"),n=n(),(0,d.Cn)(),n),o={class:"warp"},l=t((()=>(0,d._)("h2",{class:"title"},"비밀번호 찾기",-1))),s=t((()=>(0,d._)("label",{for:"email"},"가입하신 이메일을 입력해 주세요.",-1))),r=t((()=>(0,d._)("label",{for:"id"},"가입하신 아이디를 입력해주세요.",-1))),u=t((()=>(0,d._)("div",{class:"anime"},null,-1))),f=t((()=>(0,d._)("p",{class:"anime"},"전송",-1))),w=[u,f];function _(n,i,e,t,u,f){const _=(0,d.up)("router-link");return(0,d.wg)(),(0,d.iD)("div",o,[l,(0,d._)("form",null,[s,(0,d.wy)((0,d._)("input",{type:"email",name:"email",id:"email",required:"required",class:"anime","onUpdate:modelValue":i[0]||(i[0]=n=>u.findLoginPw_email=n)},null,512),[[a.nr,u.findLoginPw_email]])]),(0,d._)("form",null,[r,(0,d.wy)((0,d._)("input",{type:"text",name:"id",id:"id",required:"required",class:"anime","onUpdate:modelValue":i[1]||(i[1]=n=>u.findLoginPw_id=n)},null,512),[[a.nr,u.findLoginPw_id]])]),(0,d._)("button",{type:"submit",onClick:i[2]||(i[2]=(...i)=>n.findLoginPw&&n.findLoginPw(...i))},w),(0,d._)("p",null,[(0,d.Wm)(_,{tag:"a",to:"/login"},{default:(0,d.w5)((()=>[(0,d.Uk)("로그인 하러가기")])),_:1})])])}var c={name:"findLoginPw",data(){return{findLoginPw_email:"",findLoginPw_id:"",founded_pw:""}},methods:{async findLoginId(){try{const n="/find/loginPw",i=await fetch(n,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify({findLoginPw_email:this.findLoginPw_email,findLoginPw_id:this.findLoginPw_id})}),e=await i.json();i.ok?this.founded_pw="등록된 패스워드는 "+e.founded_pw+" 입니다.":this.founded_pw=i.statusMessage,alert(this.founded_pw)}catch(n){console.error(n)}}}},m=e(89);const p=(0,m.Z)(c,[["render",_],["__scopeId","data-v-2e62b5e4"]]);var g=p}}]);
//# sourceMappingURL=765.8ded6fe9.js.map