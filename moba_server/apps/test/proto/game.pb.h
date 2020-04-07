// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: game.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_game_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_game_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3011000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3011004 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_table_driven.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/inlined_string_field.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/generated_enum_reflection.h>
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_game_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_game_2eproto {
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTableField entries[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::AuxillaryParseTableField aux[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTable schema[2]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::FieldMetadata field_metadata[];
  static const ::PROTOBUF_NAMESPACE_ID::internal::SerializationTable serialization_table[];
  static const ::PROTOBUF_NAMESPACE_ID::uint32 offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_game_2eproto;
class LoginReq;
class LoginReqDefaultTypeInternal;
extern LoginReqDefaultTypeInternal _LoginReq_default_instance_;
class LoginRes;
class LoginResDefaultTypeInternal;
extern LoginResDefaultTypeInternal _LoginRes_default_instance_;
PROTOBUF_NAMESPACE_OPEN
template<> ::LoginReq* Arena::CreateMaybeMessage<::LoginReq>(Arena*);
template<> ::LoginRes* Arena::CreateMaybeMessage<::LoginRes>(Arena*);
PROTOBUF_NAMESPACE_CLOSE

enum Cmd : int {
  eLoginReq = 0,
  eLoginRes = 1,
  Cmd_INT_MIN_SENTINEL_DO_NOT_USE_ = std::numeric_limits<::PROTOBUF_NAMESPACE_ID::int32>::min(),
  Cmd_INT_MAX_SENTINEL_DO_NOT_USE_ = std::numeric_limits<::PROTOBUF_NAMESPACE_ID::int32>::max()
};
bool Cmd_IsValid(int value);
constexpr Cmd Cmd_MIN = eLoginReq;
constexpr Cmd Cmd_MAX = eLoginRes;
constexpr int Cmd_ARRAYSIZE = Cmd_MAX + 1;

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* Cmd_descriptor();
template<typename T>
inline const std::string& Cmd_Name(T enum_t_value) {
  static_assert(::std::is_same<T, Cmd>::value ||
    ::std::is_integral<T>::value,
    "Incorrect type passed to function Cmd_Name.");
  return ::PROTOBUF_NAMESPACE_ID::internal::NameOfEnum(
    Cmd_descriptor(), enum_t_value);
}
inline bool Cmd_Parse(
    const std::string& name, Cmd* value) {
  return ::PROTOBUF_NAMESPACE_ID::internal::ParseNamedEnum<Cmd>(
    Cmd_descriptor(), name, value);
}
// ===================================================================

class LoginReq :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:LoginReq) */ {
 public:
  LoginReq();
  virtual ~LoginReq();

  LoginReq(const LoginReq& from);
  LoginReq(LoginReq&& from) noexcept
    : LoginReq() {
    *this = ::std::move(from);
  }

  inline LoginReq& operator=(const LoginReq& from) {
    CopyFrom(from);
    return *this;
  }
  inline LoginReq& operator=(LoginReq&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const LoginReq& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const LoginReq* internal_default_instance() {
    return reinterpret_cast<const LoginReq*>(
               &_LoginReq_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  friend void swap(LoginReq& a, LoginReq& b) {
    a.Swap(&b);
  }
  inline void Swap(LoginReq* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline LoginReq* New() const final {
    return CreateMaybeMessage<LoginReq>(nullptr);
  }

  LoginReq* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<LoginReq>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const LoginReq& from);
  void MergeFrom(const LoginReq& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(LoginReq* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "LoginReq";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_game_2eproto);
    return ::descriptor_table_game_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kNameFieldNumber = 1,
    kEmailFieldNumber = 3,
    kAgeFieldNumber = 2,
    kIntSetFieldNumber = 4,
  };
  // string name = 1;
  void clear_name();
  const std::string& name() const;
  void set_name(const std::string& value);
  void set_name(std::string&& value);
  void set_name(const char* value);
  void set_name(const char* value, size_t size);
  std::string* mutable_name();
  std::string* release_name();
  void set_allocated_name(std::string* name);
  private:
  const std::string& _internal_name() const;
  void _internal_set_name(const std::string& value);
  std::string* _internal_mutable_name();
  public:

  // string email = 3;
  void clear_email();
  const std::string& email() const;
  void set_email(const std::string& value);
  void set_email(std::string&& value);
  void set_email(const char* value);
  void set_email(const char* value, size_t size);
  std::string* mutable_email();
  std::string* release_email();
  void set_allocated_email(std::string* email);
  private:
  const std::string& _internal_email() const;
  void _internal_set_email(const std::string& value);
  std::string* _internal_mutable_email();
  public:

  // int32 age = 2;
  void clear_age();
  ::PROTOBUF_NAMESPACE_ID::int32 age() const;
  void set_age(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_age() const;
  void _internal_set_age(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // int32 int_set = 4;
  void clear_int_set();
  ::PROTOBUF_NAMESPACE_ID::int32 int_set() const;
  void set_int_set(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_int_set() const;
  void _internal_set_int_set(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // @@protoc_insertion_point(class_scope:LoginReq)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr name_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr email_;
  ::PROTOBUF_NAMESPACE_ID::int32 age_;
  ::PROTOBUF_NAMESPACE_ID::int32 int_set_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  friend struct ::TableStruct_game_2eproto;
};
// -------------------------------------------------------------------

class LoginRes :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:LoginRes) */ {
 public:
  LoginRes();
  virtual ~LoginRes();

  LoginRes(const LoginRes& from);
  LoginRes(LoginRes&& from) noexcept
    : LoginRes() {
    *this = ::std::move(from);
  }

  inline LoginRes& operator=(const LoginRes& from) {
    CopyFrom(from);
    return *this;
  }
  inline LoginRes& operator=(LoginRes&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const LoginRes& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const LoginRes* internal_default_instance() {
    return reinterpret_cast<const LoginRes*>(
               &_LoginRes_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    1;

  friend void swap(LoginRes& a, LoginRes& b) {
    a.Swap(&b);
  }
  inline void Swap(LoginRes* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline LoginRes* New() const final {
    return CreateMaybeMessage<LoginRes>(nullptr);
  }

  LoginRes* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<LoginRes>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const LoginRes& from);
  void MergeFrom(const LoginRes& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(LoginRes* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "LoginRes";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_game_2eproto);
    return ::descriptor_table_game_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kStatusFieldNumber = 1,
  };
  // int32 status = 1;
  void clear_status();
  ::PROTOBUF_NAMESPACE_ID::int32 status() const;
  void set_status(::PROTOBUF_NAMESPACE_ID::int32 value);
  private:
  ::PROTOBUF_NAMESPACE_ID::int32 _internal_status() const;
  void _internal_set_status(::PROTOBUF_NAMESPACE_ID::int32 value);
  public:

  // @@protoc_insertion_point(class_scope:LoginRes)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::int32 status_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  friend struct ::TableStruct_game_2eproto;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// LoginReq

// string name = 1;
inline void LoginReq::clear_name() {
  name_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline const std::string& LoginReq::name() const {
  // @@protoc_insertion_point(field_get:LoginReq.name)
  return _internal_name();
}
inline void LoginReq::set_name(const std::string& value) {
  _internal_set_name(value);
  // @@protoc_insertion_point(field_set:LoginReq.name)
}
inline std::string* LoginReq::mutable_name() {
  // @@protoc_insertion_point(field_mutable:LoginReq.name)
  return _internal_mutable_name();
}
inline const std::string& LoginReq::_internal_name() const {
  return name_.GetNoArena();
}
inline void LoginReq::_internal_set_name(const std::string& value) {
  
  name_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void LoginReq::set_name(std::string&& value) {
  
  name_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:LoginReq.name)
}
inline void LoginReq::set_name(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  
  name_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:LoginReq.name)
}
inline void LoginReq::set_name(const char* value, size_t size) {
  
  name_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:LoginReq.name)
}
inline std::string* LoginReq::_internal_mutable_name() {
  
  return name_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* LoginReq::release_name() {
  // @@protoc_insertion_point(field_release:LoginReq.name)
  
  return name_.ReleaseNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void LoginReq::set_allocated_name(std::string* name) {
  if (name != nullptr) {
    
  } else {
    
  }
  name_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), name);
  // @@protoc_insertion_point(field_set_allocated:LoginReq.name)
}

// int32 age = 2;
inline void LoginReq::clear_age() {
  age_ = 0;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginReq::_internal_age() const {
  return age_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginReq::age() const {
  // @@protoc_insertion_point(field_get:LoginReq.age)
  return _internal_age();
}
inline void LoginReq::_internal_set_age(::PROTOBUF_NAMESPACE_ID::int32 value) {
  
  age_ = value;
}
inline void LoginReq::set_age(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_age(value);
  // @@protoc_insertion_point(field_set:LoginReq.age)
}

// string email = 3;
inline void LoginReq::clear_email() {
  email_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline const std::string& LoginReq::email() const {
  // @@protoc_insertion_point(field_get:LoginReq.email)
  return _internal_email();
}
inline void LoginReq::set_email(const std::string& value) {
  _internal_set_email(value);
  // @@protoc_insertion_point(field_set:LoginReq.email)
}
inline std::string* LoginReq::mutable_email() {
  // @@protoc_insertion_point(field_mutable:LoginReq.email)
  return _internal_mutable_email();
}
inline const std::string& LoginReq::_internal_email() const {
  return email_.GetNoArena();
}
inline void LoginReq::_internal_set_email(const std::string& value) {
  
  email_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void LoginReq::set_email(std::string&& value) {
  
  email_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:LoginReq.email)
}
inline void LoginReq::set_email(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  
  email_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:LoginReq.email)
}
inline void LoginReq::set_email(const char* value, size_t size) {
  
  email_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:LoginReq.email)
}
inline std::string* LoginReq::_internal_mutable_email() {
  
  return email_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* LoginReq::release_email() {
  // @@protoc_insertion_point(field_release:LoginReq.email)
  
  return email_.ReleaseNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void LoginReq::set_allocated_email(std::string* email) {
  if (email != nullptr) {
    
  } else {
    
  }
  email_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), email);
  // @@protoc_insertion_point(field_set_allocated:LoginReq.email)
}

// int32 int_set = 4;
inline void LoginReq::clear_int_set() {
  int_set_ = 0;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginReq::_internal_int_set() const {
  return int_set_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginReq::int_set() const {
  // @@protoc_insertion_point(field_get:LoginReq.int_set)
  return _internal_int_set();
}
inline void LoginReq::_internal_set_int_set(::PROTOBUF_NAMESPACE_ID::int32 value) {
  
  int_set_ = value;
}
inline void LoginReq::set_int_set(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_int_set(value);
  // @@protoc_insertion_point(field_set:LoginReq.int_set)
}

// -------------------------------------------------------------------

// LoginRes

// int32 status = 1;
inline void LoginRes::clear_status() {
  status_ = 0;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginRes::_internal_status() const {
  return status_;
}
inline ::PROTOBUF_NAMESPACE_ID::int32 LoginRes::status() const {
  // @@protoc_insertion_point(field_get:LoginRes.status)
  return _internal_status();
}
inline void LoginRes::_internal_set_status(::PROTOBUF_NAMESPACE_ID::int32 value) {
  
  status_ = value;
}
inline void LoginRes::set_status(::PROTOBUF_NAMESPACE_ID::int32 value) {
  _internal_set_status(value);
  // @@protoc_insertion_point(field_set:LoginRes.status)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__
// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)


PROTOBUF_NAMESPACE_OPEN

template <> struct is_proto_enum< ::Cmd> : ::std::true_type {};
template <>
inline const EnumDescriptor* GetEnumDescriptor< ::Cmd>() {
  return ::Cmd_descriptor();
}

PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_game_2eproto
